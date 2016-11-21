using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using RTH.BusDrivers;
using RTH.Modeo2;

namespace BusDriverTest
{
    [TestClass]
    public class UnitTest1
    {
        ProblemStatement TestProblem;
        Schedule TestSchedule;

        [TestInitialize]
        public void Initialize()
        {
            TestProblem = new ProblemStatement()
            {
                NumDays = 3,
                NumLines = 3,
                Drivers = new System.Collections.Generic.List<Driver>()
            };
            TestProblem.Drivers.Add(new Driver("D1", TestProblem.NumDays, TestProblem.NumLines)
                .AddDayOff(1, 2)
                .AddLine(1)
                .AddPrefShift(1, 3)
            );
            TestProblem.Drivers.Add(new Driver("D2", TestProblem.NumDays, TestProblem.NumLines)
                .AddDayOff(2)
                .AddLine(2, 3)
                .AddPrefShift(1, 1)
            );
            TestProblem.Drivers.Add(new Driver("D3", TestProblem.NumDays, TestProblem.NumLines)
                .AddDayOff(3)
                .AddLine(2)
                .AddPrefShift(2, 2)
            );
            TestProblem.Drivers.Add(new Driver("D4", TestProblem.NumDays, TestProblem.NumLines)
                .AddDayOff(1, 3)
                .AddLine(1, 3)
                .AddPrefShift(1, 2)
            );

            TestProblem.Drivers.Add(new Driver("D5", TestProblem.NumDays, TestProblem.NumLines)
                //.AddDayOff(1, 3)
                .AddLine(1, 2, 3)
                .AddPrefShift(1, 1, 2)
                .AddPrefShift(2, 3)
            );
            TestProblem.Drivers.Add(new Driver("D6", TestProblem.NumDays, TestProblem.NumLines)
                            //.AddDayOff(1, 3)
                            .AddLine(2, 3)
                            //.AddPrefShift(1, 1, 2)
                            //.AddPrefShift(2, 3)
                        );
            TestProblem.Drivers.Add(new Driver("D7", TestProblem.NumDays, TestProblem.NumLines)
                            //.AddDayOff(1, 3)
                            .AddLine(1, 2)
                            .AddPrefShift(1, 1, 2)
                            .AddPrefShift(2, 3)
                            .AddPrefDayOff(2)
                        );
            TestProblem.Drivers.Add(new Driver("D8", TestProblem.NumDays, TestProblem.NumLines)
                            //.AddDayOff(1, 3)
                            .AddLine(1, 3)
                            //.AddPrefShift(1, 1, 2)
                            //.AddPrefShift(2, 3)
                        );

            TestSchedule = new Schedule(TestProblem.Drivers.ToArray(), TestProblem.NumDays, TestProblem.NumLines);
        }

        [TestMethod]
        public void DriverSetupMethods()
        {
            var d = TestProblem.Drivers[3];
            Assert.AreEqual(d.Name, "D4");
            Assert.IsTrue(d.DaysOff[0] && d.DaysOff[2]);
            Assert.IsFalse(d.DaysOff[1]);
            Assert.AreEqual(d.Lines.Count, 2);
            Assert.IsTrue(d.Lines.Contains(0) && d.Lines.Contains(2));
            Assert.IsFalse(d.Lines.Contains(1));
        }

        [TestMethod]
        public void ScheduleMethods()
        {
            Assert.IsTrue(TestSchedule.GetAssignments().Where(a => a.Driver != null).Count() == 0);

            var d2 = TestProblem.Drivers.Single(d => d.Name == "D2");
            Assert.IsNotNull(d2);
            Assert.IsTrue(TestSchedule.EmptyAssignments().Count() == 18);
            Assert.IsFalse(TestSchedule.DriverSchedule(d2)[0]);
            Assert.IsFalse(TestSchedule.DriverSchedule(d2)[5]);
            Assert.IsFalse(TestSchedule.WorkingOn(d2, 0));
            Assert.IsFalse(TestSchedule.WorkingOn(d2, 1));
            Assert.IsFalse(TestSchedule.WorkingOn(d2, 2));

            Assert.IsTrue(TestSchedule.SetShift(0, 0, 1, d2));
            Assert.IsTrue(TestSchedule.GetAssignments().Where(a => a.Driver != null).Count() == 1);
            Assert.IsTrue(TestSchedule.DriverSchedule(d2)[0]);
            Assert.IsFalse(TestSchedule.DriverSchedule(d2)[5]);
            Assert.IsTrue(TestSchedule.EmptyAssignments().Count() == 17);
            Assert.IsTrue(TestSchedule.WorkingOn(d2, 0));
            Assert.IsFalse(TestSchedule.WorkingOn(d2, 1));
            Assert.IsFalse(TestSchedule.WorkingOn(d2, 2));

            // day off
            Assert.IsFalse(TestSchedule.SetShift(1, 0, 1, d2));
            Assert.IsTrue(TestSchedule.GetAssignments().Where(a => a.Driver != null).Count() == 1);

            // wrong line
            Assert.IsFalse(TestSchedule.SetShift(0, 0, 0, d2));
            Assert.IsTrue(TestSchedule.GetAssignments().Where(a => a.Driver != null).Count() == 1);

            // aleady on that day
            Assert.IsFalse(TestSchedule.SetShift(0, 1, 0, d2));
            Assert.IsTrue(TestSchedule.GetAssignments().Where(a => a.Driver != null).Count() == 1);

            // schedule other day
            Assert.IsTrue(TestSchedule.SetShift(2, 1, 1, d2));
            Assert.IsTrue(TestSchedule.GetAssignments().Where(a => a.Driver != null).Count() == 2);
            Assert.IsTrue(TestSchedule.EmptyAssignments().Count() == 16);
            Assert.IsTrue(TestSchedule.DriverSchedule(d2)[5]);
            Assert.IsTrue(TestSchedule.WorkingOn(d2, 0));
            Assert.IsFalse(TestSchedule.WorkingOn(d2, 1));
            Assert.IsTrue(TestSchedule.WorkingOn(d2, 2));

            //remove shift
            TestSchedule.SetShift(0, 0, 1, null);
            Assert.IsTrue(TestSchedule.GetAssignments().Where(a => a.Driver != null).Count() == 1);
            Assert.IsTrue(TestSchedule.EmptyAssignments().Count() == 17);
            Assert.IsFalse(TestSchedule.DriverSchedule(d2)[0]);
            Assert.IsFalse(TestSchedule.WorkingOn(d2, 0));
            Assert.IsFalse(TestSchedule.WorkingOn(d2, 1));
            Assert.IsTrue(TestSchedule.WorkingOn(d2, 2));



        }

        [TestMethod]
        public void TestObjectives()
        {
            var result = true;
            result = TestSchedule.SetShift(new Assignment() { Driver = TestProblem.Drivers[0], Day = 2, Shift = 0, Line = 0 }); //pref shift
            Assert.IsTrue(result);
            result = TestSchedule.SetShift(new Assignment() { Driver = TestProblem.Drivers[1], Day = 0, Shift = 0, Line = 1 }); //pref shift
            Assert.IsTrue(result);
            result = TestSchedule.SetShift(new Assignment() { Driver = TestProblem.Drivers[1], Day = 2, Shift = 0, Line = 2 });
            Assert.IsTrue(result);
            result = TestSchedule.SetShift(new Assignment() { Driver = TestProblem.Drivers[2], Day = 0, Shift = 1, Line = 1 });
            Assert.IsTrue(result);
            result = TestSchedule.SetShift(new Assignment() { Driver = TestProblem.Drivers[2], Day = 1, Shift = 0, Line = 1 });
            Assert.IsTrue(result);
            result = TestSchedule.SetShift(new Assignment() { Driver = TestProblem.Drivers[3], Day = 1, Shift = 0, Line = 2 }); //pref shift
            Assert.IsTrue(result);
            // try to assign to a day that driver is already working
            result = TestSchedule.SetShift(new Assignment() { Driver = TestProblem.Drivers[3], Day = 1, Shift = 1, Line = 2 });
            Assert.IsFalse(result);

            // D1: XXXX1.
            // D2: 2.XX3.
            // D3: .22.XX -- late followed by early
            // D4: XX3.XX


            var assignments = TestSchedule.GetAssignments();
            Assert.IsTrue(assignments.Count() == 18);
            Assert.IsTrue(assignments.Where(a => a.Driver != null).Count() == 6);
            Assert.IsTrue(TestSchedule.EmptyAssignments().Count() == 12);

            Assert.IsTrue(Objectives.UnassignedShifts(TestSchedule) == 12);
            Assert.IsTrue(Objectives.ShiftPrefs(TestSchedule) == 3);
            Assert.AreEqual(1, Objectives.DayOffPrefs(TestSchedule)); // D7
            Assert.IsTrue(Objectives.LateFollowedByEarly(TestSchedule) == 1);

            //Assert.AreEqual(0, Objectives.DayOffPrefs(TestSchedule));

            result = TestSchedule.SetShift(new Assignment() { Driver = TestProblem.Drivers[4], Day = 0, Shift = 0, Line = 0 }); //pref shift
            Assert.IsTrue(result);
            result = TestSchedule.SetShift(new Assignment() { Driver = TestProblem.Drivers[4], Day = 1, Shift = 0, Line = 0 }); //pref shift
            Assert.IsTrue(result);
            result = TestSchedule.SetShift(new Assignment() { Driver = TestProblem.Drivers[4], Day = 2, Shift = 1, Line = 2 }); //pref shift
            Assert.IsTrue(result);

            // D1: XXXX1.
            // D2: 2.XX3.
            // D3: .22.XX -- late followed by early
            // D4: XX3.XX
            // D5: 1.1..3

            assignments = TestSchedule.GetAssignments();
            Assert.IsTrue(assignments.Count() == 18);
            Assert.IsTrue(assignments.Where(a => a.Driver != null).Count() == 9);
            Assert.IsTrue(TestSchedule.EmptyAssignments().Count() == 9);

            Assert.IsTrue(Objectives.UnassignedShifts(TestSchedule) == 9);
            Assert.IsTrue(Objectives.ShiftPrefs(TestSchedule) == 6);
            Assert.AreEqual(1, Objectives.DayOffPrefs(TestSchedule)); // D7
            Assert.IsTrue(Objectives.LateFollowedByEarly(TestSchedule) == 1);

            Assert.IsTrue(Objectives.ExcessLateShifts(TestSchedule, 1) == 0);
            Assert.IsTrue(Objectives.ExcessLateShifts(TestSchedule, 2) == 0);
            Assert.IsTrue(Objectives.ExcessLateShifts(TestSchedule, 3) == 0);

            result = TestSchedule.SetShift(new Assignment() { Driver = TestProblem.Drivers[5], Day = 0, Shift = 0, Line = 2 }); 
            Assert.IsTrue(result);
            result = TestSchedule.SetShift(new Assignment() { Driver = TestProblem.Drivers[5], Day = 1, Shift = 1, Line = 2 }); 
            Assert.IsTrue(result);
            result = TestSchedule.SetShift(new Assignment() { Driver = TestProblem.Drivers[5], Day = 2, Shift = 0, Line = 1 }); 
            Assert.IsTrue(result);

            result = TestSchedule.SetShift(new Assignment() { Driver = TestProblem.Drivers[6], Day = 0, Shift = 1, Line = 0 }); // shift pref
            Assert.IsTrue(result);
            result = TestSchedule.SetShift(new Assignment() { Driver = TestProblem.Drivers[6], Day = 1, Shift = 1, Line = 1 });
            Assert.IsTrue(result);
            result = TestSchedule.SetShift(new Assignment() { Driver = TestProblem.Drivers[6], Day = 2, Shift = 1, Line = 1 });
            Assert.IsTrue(result);

            result = TestSchedule.SetShift(new Assignment() { Driver = TestProblem.Drivers[7], Day = 0, Shift = 1, Line = 2 });
            Assert.IsTrue(result);
            result = TestSchedule.SetShift(new Assignment() { Driver = TestProblem.Drivers[7], Day = 1, Shift = 1, Line = 0 });
            Assert.IsTrue(result);
            result = TestSchedule.SetShift(new Assignment() { Driver = TestProblem.Drivers[7], Day = 2, Shift = 1, Line = 0 });
            Assert.IsTrue(result);

            // D1: XXXX1.
            // D2: 2.XX3.
            // D3: .22.XX -- late followed by early
            // D4: XX3.XX
            // D5: 1.1..3
            // D6: 3..32. -- late followed by early
            // D7: .1.2.2 -- 3 lates
            // D8: .3.1.1 -- 3 lates

            assignments = TestSchedule.GetAssignments();
            Assert.IsTrue(assignments.Count() == 18);
            Assert.IsTrue(assignments.Where(a => a.Driver != null).Count() == 18);
            Assert.IsTrue(TestSchedule.EmptyAssignments().Count() == 0);

            Assert.IsTrue(Objectives.UnassignedShifts(TestSchedule) == 0);
            Assert.AreEqual(7, Objectives.ShiftPrefs(TestSchedule));
            Assert.AreEqual(0, Objectives.DayOffPrefs(TestSchedule)); // D7
            Assert.IsTrue(Objectives.LateFollowedByEarly(TestSchedule) == 2);

            Assert.IsTrue(Objectives.ExcessLateShifts(TestSchedule, 1) == 4);
            Assert.IsTrue(Objectives.ExcessLateShifts(TestSchedule, 2) == 2);
            Assert.IsTrue(Objectives.ExcessLateShifts(TestSchedule, 3) == 0);

            Assert.IsTrue(Objectives.ConsecutiveLateShifts(TestSchedule, 1) == 4);
            Assert.IsTrue(Objectives.ConsecutiveLateShifts(TestSchedule, 2) == 2);
            Assert.IsTrue(Objectives.ConsecutiveLateShifts(TestSchedule, 3) == 0);

            result = TestSchedule.SetShift(1, 1, 1, null); // remove D7 from shift
            Assert.IsTrue(result);
            Assert.IsTrue(TestSchedule.EmptyAssignments().Count() == 1);

            Assert.AreEqual(1, Objectives.DayOffPrefs(TestSchedule));

            Assert.AreEqual(3, Objectives.ExcessLateShifts(TestSchedule, 1));
            Assert.AreEqual(1, Objectives.ExcessLateShifts(TestSchedule, 2));
            Assert.AreEqual(0, Objectives.ExcessLateShifts(TestSchedule, 3));

            Assert.AreEqual(2, Objectives.ConsecutiveLateShifts(TestSchedule, 1));
            Assert.AreEqual(1, Objectives.ConsecutiveLateShifts(TestSchedule, 2));
            Assert.AreEqual(0, Objectives.ConsecutiveLateShifts(TestSchedule, 3));

        }
        [TestMethod]
        public void TestLongRest()
        {
            foreach (var driver in TestProblem.Drivers) driver.Resize(7);

            var LongRestTestProblem = new ProblemStatement()
            {
                Drivers = TestProblem.Drivers,
                NumDays = 7,
                NumLines = 1
            };
            var LongRestTestSchedule = new Schedule(LongRestTestProblem);

            var drivers = LongRestTestProblem.DriversForLine(0);
            Assert.AreEqual(5, drivers.Count());
            var d = drivers.ToArray();
            var d1 = d[0];
            var d4 = d[1];
            var d5 = d[2];
            var d7 = d[3];
            var d8 = d[4];
            LongRestTestSchedule.SetShift(0, 0, 0, d5);
            LongRestTestSchedule.SetShift(0, 1, 0, d7);
            LongRestTestSchedule.SetShift(1, 0, 0, d5);
            LongRestTestSchedule.SetShift(1, 1, 0, d8);
            LongRestTestSchedule.SetShift(2, 0, 0, d1);
            LongRestTestSchedule.SetShift(2, 1, 0, d8);
            LongRestTestSchedule.SetShift(3, 0, 0, d1);
            LongRestTestSchedule.SetShift(3, 1, 0, d4);
            LongRestTestSchedule.SetShift(4, 0, 0, d4);
            LongRestTestSchedule.SetShift(4, 1, 0, d1);
            LongRestTestSchedule.SetShift(5, 0, 0, d7);
            LongRestTestSchedule.SetShift(5, 1, 0, d4);
            LongRestTestSchedule.SetShift(6, 0, 0, d8);
            LongRestTestSchedule.SetShift(6, 1, 0, d5);

            // L1: 57 58 18 14 41 74 85
            // D1: XX XX 1. 1. .1 .. ..
            // D2: .. XX .. .. .. .. ..
            // D3: .. .. XX .. .. .. ..
            // D4: .. .. .. .1 1. .1 ..
            // D5: 1. 1. .. .. .. .. .1
            // D6: .. .. .. .. .. .. ..
            // D7: .1 .. .. .. .. 1. ..
            // D8: .. .1 .1 .. .. .. 1.

            Assert.AreEqual(0, LongRestTestSchedule.EmptyAssignments().Count());
            Assert.AreEqual(1, Objectives.LateFollowedByEarly(LongRestTestSchedule));

            CollectionAssert.AreEqual(new bool[] { false, false, false, false, true, false, true, false, false, true, false, false, false, false }, 
                LongRestTestSchedule.DriverSchedule(d1));

            Assert.AreEqual(2, Objectives.LongRest(LongRestTestSchedule, 2, d1));
            Assert.AreEqual(0, Objectives.LongRest(LongRestTestSchedule, 3, d1));
            Assert.AreEqual(1, Objectives.LongRest(LongRestTestSchedule, 2, d4));
            Assert.AreEqual(1, Objectives.LongRest(LongRestTestSchedule, 3, d4));
            Assert.AreEqual(0, Objectives.LongRest(LongRestTestSchedule, 4, d4));

            // drivers 2, 3 and 6 each will contribute one long rest
            Assert.AreEqual(9, Objectives.LongRest(LongRestTestSchedule, 2));
            Assert.AreEqual(7, Objectives.LongRest(LongRestTestSchedule, 3));
            Assert.AreEqual(5, Objectives.LongRest(LongRestTestSchedule, 4));
            Assert.AreEqual(3, Objectives.LongRest(LongRestTestSchedule, 5));
        }
    }
}
