using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using RTH.BusDrivers;
using RTH.Modeo2;

namespace BusDriverTest
{
    [TestClass]
    public class UnitTest1
    {
        ProblemStatement Problem;

        [TestInitialize]
        public void Initialize()
        {
            Problem = new ProblemStatement()
            {
                NumDays = 3,
                NumLines = 3,
                Drivers = new System.Collections.Generic.List<Driver>()
            };
            Problem.Drivers.Add(new Driver("D1")
                .AddDayOff(1, 2)
                .AddLine(1)
                .AddPrefShift(1, 1)
            );
            Problem.Drivers.Add(new Driver("D2")
                .AddDayOff(2)
                .AddLine(2, 3)
                .AddPrefShift(1, 2)
            );
            Problem.Drivers.Add(new Driver("D3")
                .AddDayOff(3)
                .AddLine(2)
                .AddPrefShift(2, 3)
            );
            Problem.Drivers.Add(new Driver("D4")
                .AddDayOff(1, 3)
                .AddLine(1, 3)
                .AddPrefShift(1, 1)
                .AddPrefShift(2, 2)
            );


        }

        [TestMethod]
        public void DriverSetupMethods()
        {
            var d = Problem.Drivers[3];
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
            var sched = new Schedule(Problem.Drivers.ToArray(), Problem.NumDays, Problem.NumLines);
            
            Assert.IsTrue(sched.GetAssignments().Where(a => a.Driver != null).Count() == 0);

            var d2 = Problem.Drivers.Single(d => d.Name == "D2");
            Assert.IsNotNull(d2);

            Assert.IsTrue(sched.SetShift(0, 0, 1, d2));
            Assert.IsTrue(sched.GetAssignments().Where(a => a.Driver != null).Count() == 1);

            // day off
            Assert.IsFalse(sched.SetShift(1, 0, 1, d2));
            Assert.IsTrue(sched.GetAssignments().Where(a => a.Driver != null).Count() == 1);

            // wrong line
            Assert.IsFalse(sched.SetShift(0, 0, 0, d2));
            Assert.IsTrue(sched.GetAssignments().Where(a => a.Driver != null).Count() == 1);

            // aleady on that day
            Assert.IsFalse(sched.SetShift(0, 1, 0, d2));
            Assert.IsTrue(sched.GetAssignments().Where(a => a.Driver != null).Count() == 1);

            // schedule other day
            Assert.IsTrue(sched.SetShift(2, 1, 1, d2));
            Assert.IsTrue(sched.GetAssignments().Where(a => a.Driver != null).Count() == 2);



        }
    }
}
