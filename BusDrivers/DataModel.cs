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
    }

    public class Assignment
    {
        public Driver Driver;
        public int Day;
        public int Shift;
        public int Line;
    }

    public class Schedule : BaseSolution
    {
        private Driver[] drivers;
        private int numLines;

        private Driver[,] shifts;
        private IList<Assignment> assignments;

        public Schedule(Driver[] Drivers, int Days = 14, int NumLines = 3)
        {
            drivers = Drivers;
            numLines = NumLines;
            shifts = new Driver [2 * Days, numLines];
        }

        public void SetShift(int day, int shift, int line, Driver d)
        {
            shifts[day * 2 + shift, line] = d;
            assignments = null;
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

                for (int day = 0; day < shifts.GetLength(0); day++)
                {
                    for (int line = 0; line < shifts.GetLength(1); line++)
                    {
                        assignments.Add(new Assignment() { Day = day, Line = line, Shift = 0, Driver = shifts[day * 2, line] });
                        assignments.Add(new Assignment() { Day = day, Line = line, Shift = 1, Driver = shifts[day * 2 + 1, line] });
                    }
                }
            }
            return assignments;
        }
    }
}
