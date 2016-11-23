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
            return "Day " + Day + " Shift " + Shift + " Line " + Line + " " + Driver?.Name;
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

        public Schedule Copy()
        {
            var s = new Schedule(drivers, days, numLines);
            Array.Copy(this.shifts, s.shifts, s.shifts.Length);
            return s;
        }
        public override bool IsDuplicate(ISolution soln)
        {
            // assume Drivers, days and lines are the same
            //compare shifts array
            var otherShifts = (soln as Schedule).GetShifts();
            for (int i=0; i<shifts.GetLength(0); i++)
            {
                for (int j=0; j<shifts.GetLength(1); j++)
                {
                    if (otherShifts[i, j] != shifts[i, j]) return false;
                }
            }
            return true;
        }

        public bool SetShift(int day, int shift, int line, Driver d)
        {
            return SetShift(day * 2 + shift, line, d);
        }
        internal bool SetShift(int index, int line, Driver d)    
        {
            var day = index / 2;
            
            var prevDriver = shifts[index, line];
            if (d != null)
            {
                // do not use scheduled day off
                if (d.DaysOff[day]) return false;

                // is driver already scheduled on this day?
                if (WorkingOn(d, day)) return false;

                // do not permit driver to be assigned to a line not trained for
                if (!d.Lines.Contains(line)) return false;

                driverSchedules[d] = null;
            }
            if (prevDriver != null)
            {
                driverSchedules[prevDriver] = null;
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

        public Driver GetDriverByName(string name)
        {
            return GetDrivers().Where(d => d.Name == name).Single();
        }
        public Driver[,] GetShifts()
        {
            return shifts;
        }

        public IList<Assignment> GetAssignments()
        {
            bool cache = true;

            if (!cache || assignments == null)
            {
                GenerateAssignments();
            }

            return assignments;
        }

        public void GenerateAssignments()
        {
            assignments = new List<Assignment>(shifts.Length);

            for (int shift = 0; shift < shifts.GetLength(0); shift += 2)
            {
                for (int line = 0; line < shifts.GetLength(1); line++)
                {
                    assignments.Add(new Assignment() { Day = shift / 2, Line = line, Shift = 0, Driver = shifts[shift, line] });
                    assignments.Add(new Assignment() { Day = shift / 2, Line = line, Shift = 1, Driver = shifts[shift + 1, line] });
                }
            }
        }

        public IEnumerable<Assignment> EmptyAssignments()
        {
            var list = GetAssignments();
            return list.Where(a => a.Driver == null);
        }

        public bool[] DriverSchedule(Driver d)
        {
            if (driverSchedules.TryGetValue(d, out var retval))
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
                retval += Line(d, day, 0);
                retval += Line(d, day, 1);
            }
            return retval;
        }

        private string Line(Driver d, int day, int sh)
        {
            var sched = DriverSchedule(d);
            var index = day * 2 + sh;
            if (!sched[index]) return ".";
            for (int i=0; i < shifts.GetLength(1);i++)
            {
                if (shifts[index, i] == d) return String.Format("{0}", i);
            }
            return "?";
        }

        public int Booked(Driver d, int day, int sh)
        {
            var index = day * 2 + sh;
            return Booked(d, index);
        }

        public int Booked(Driver d, int index)
        {
            var count = 0;
            
            for (int i = 0; i < shifts.GetLength(1); i++)
            {
                if (shifts[index, i] == d) count++;
            }
            return count;
        }
        public bool BookedOnDayOff(Driver d)
        {
            for (int day = 0; day < days; day++)
            {
                if (d.DaysOff[day] && WorkingOn(d, day)) return true;
            }
            return false;
            
        }
        public bool BookingViolation()
        {
            foreach (Driver d in GetDrivers())
            {
                if (BookedOnDayOff(d))
                    return true;

                for (int index = 0; index < shifts.GetLength(0); index++)
                {
                    if (Booked(d, index) > 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override String ToString()
        {
            return "Schedule "+ base.GetHashCode();
        }

       
    }
}
