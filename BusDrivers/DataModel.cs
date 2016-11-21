using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RTH.Modeo2;

namespace RTH.BusDrivers
{
    public class Driver
    {
        public string Name;
        public List<int> Lines;
        public bool[] DaysOff; // 0 - 13
        public List<int> PrefDaysOff; // 0 - 13
        public int[] PrefShift; //0, 1, 2 for each day

        public Driver(string name, int Days = 14, int NumLines = 3)
        {
            Name = name;
            DaysOff = new bool[Days];
            PrefShift = new int[Days];
            PrefDaysOff = new List<int>();
            Lines = new List<int>();
        }

        public Driver AddLine(int line) { Lines.Add(line-1); return this; }

        public Driver AddLine(params int[] lines) { lines.ToList().ForEach(line => AddLine(line)); return this; }

        public Driver AddDayOff(int day) { DaysOff[day-1] = true; return this; }

        public Driver AddDayOff(params int[] days) { days.ToList().ForEach(day => AddDayOff(day)); return this; }

        public Driver AddPrefDayOff(int day) { PrefDaysOff.Add(day-1); return this; }

        public Driver AddPrefDayOff(params int[] days) { days.ToList().ForEach(day => AddPrefDayOff(day)); return this; }
        
        public Driver AddPrefShift(int shift, int day) { PrefShift[day-1] = shift; return this; }

        public Driver AddPrefShift(int shift, params int[] days) { days.ToList().ForEach(day => AddPrefShift(shift, day)); return this; }

        public override String ToString()
        {
            return base.ToString();
        }

        public void Resize(int NumDays)
        {
            Array.Resize(ref DaysOff, NumDays);
            Array.Resize(ref PrefShift, NumDays);
        }
    }

    public class Assignment
    {
        public Driver Driver;
        public int Day;
        public int Shift;
        public int Line;

        public override String ToString()
        {
            return Driver.Name + " Day " + Day + " Shift " + Shift + " Line " + Line;
        }
    }

    public class Schedule : BaseSolution
    {
        private Driver[] drivers;
        private int numLines;
        private int days;

        private Driver[,] shifts;
        private IList<Assignment> assignments;
        private Dictionary<Driver, bool[]> driverSchedules = new Dictionary<Driver, bool[]>();

        public Schedule(Driver[] Drivers, int Days = 14, int NumLines = 3)
        {
            drivers = Drivers;
            numLines = NumLines;
            days = Days;
            shifts = new Driver [2 * Days, numLines];
        }

        public Schedule(ProblemStatement ps) 
            : this(ps.Drivers.ToArray(), ps.NumDays, ps.NumLines) { }

        public bool SetShift(int day, int shift, int line, Driver d)
        {
            var index = day * 2 + shift;
            var prevDriver = shifts[index, line];
            if (prevDriver != null)
            {
                driverSchedules[prevDriver] = null;
            }
            if (d != null)
            {
                // do not use scheduled day off
                if (d.DaysOff[day]) return false;

                // do not permit driver to work both shifts on a day
                var checkshift = 1 - shift;
                var s = DriverSchedule(d);
                if (s[day * 2 + checkshift]) return false;

                // do not permit driver to be assigned to a line not trained for
                if (!d.Lines.Contains(line)) return false;

                driverSchedules[d] = null;
            }

            shifts[index, line] = d;

            // reset cached data structures
            assignments = null;
            return true;
        }

        public bool SetShift(Assignment a)
        {
            return SetShift(a.Day, a.Shift, a.Line, a.Driver);
        }

        public Driver[] GetDrivers()
        {
            return drivers;
        }

        public Driver[,] GetShifts()
        {
            return shifts;
        }

        public IList<Assignment> GetAssignments()
        {
            if (assignments == null)
            {
                assignments = new List<Assignment>(shifts.Length);

                for (int shift = 0; shift < shifts.GetLength(0); shift+=2)
                {
                    for (int line = 0; line < shifts.GetLength(1); line++)
                    {
                        assignments.Add(new Assignment() { Day = shift/2, Line = line, Shift = 0, Driver = shifts[shift, line] });
                        assignments.Add(new Assignment() { Day = shift/2, Line = line, Shift = 1, Driver = shifts[shift + 1, line] });
                    }
                }
            }
            return assignments;
        }

        public IEnumerable<Assignment> EmptyAssignments()
        {
            var list = GetAssignments();
            return list.Where(a => a.Driver == null);
        }

        public bool[] DriverSchedule(Driver d)
        {
            bool[] retval;
            if (driverSchedules.TryGetValue(d, out retval))
            {
                if (retval != null) return retval;
            }
            retval = new bool[shifts.GetLength(0)];
            for (int shift=0; shift < retval.Length; shift++)
            {
                for (var line = 0; line < shifts.GetLength(1); line++)
                {
                    if (d.Equals(shifts[shift,line]))
                    {
                        retval[shift] = true;
                    }
                }
            }
            driverSchedules[d] = retval;
            return retval;
        }

        public bool WorkingOn(Driver d, int day)
        {
            var s = DriverSchedule(d);
            return s[day * 2] || s[day * 2 + 1];
        }

        public string DisplayDriver(Driver d)
        {
            string retval = d.Name + ": ";
            var sched = DriverSchedule(d);
            for (var day = 0; day < days; day++ )
            {
                if (d.DaysOff[day]) {
                    retval = retval + "XX";
                    continue;
                }
                retval += sched[day * 2] ? "*" : ".";
                retval += sched[day * 2 + 1] ? "*" : ".";
            }
            return retval;
        }
    }
}
