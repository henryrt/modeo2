using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RTH.Modeo2;

namespace RTH.BusDrivers
{
    public class Objectives
    {
        public static double UnassignedShifts(ISolution soln)
        {
            var schedule = soln as Schedule;
            return schedule.EmptyAssignments().Count() * 1.0;
        }

        public static double ShiftPrefs(ISolution soln)
        {
            var schedule = soln as Schedule;
            var assignments = schedule.GetAssignments();
            var count = 0;
            foreach (var a in assignments)
            {
                // check if driver has a preferred shift on that day
                if (a.Driver?.PrefShift[a.Day] == a.Shift + 1) count++;
            }
            return count;
        }

        public static double ExcessLateShifts(ISolution soln)
        {
            // how many excess late shifts are there? Each driver should have no more than 4
            const int limit = 4;
            var schedule = soln as Schedule;
            var shifts = schedule.GetShifts();
            var lates = new Dictionary<Driver, int>();

            for (var shift = 1; shift < shifts.GetLength(0); shift += 2)
            {
                for (var line = 0; line < shifts.GetLength(1); line++)
                {
                    var d = shifts[shift, line];
                    if (d != null)
                    {
                        int i;
                        if (!lates.TryGetValue(d, out i)) i = 0;
                        lates[d] = i + 1;
                    }
                }
            }

            var retval = 0;
            foreach (var count in lates.Values)
            {
                if (count > limit) retval += (count - limit);
            }
            return retval;
        }

        public static double ConsecutiveLateShifts(ISolution soln)
        {
            const int limit = 3;
            var schedule = soln as Schedule;
            var drivers = schedule.GetDrivers();

            var retval = 0;
            foreach (Driver d in drivers)
            {
                var s = schedule.DriverSchedule(d);

                var consec = 0;
                for (var shift = 1; shift < s.Length; shift += 2) // only night shifts
                {
                    if (s[shift])
                    {
                        consec++;
                        if (consec > limit) retval++;
                    }
                    else consec = 0;
                }
            }

            return retval;
        }
        public static double LateFollowedByEarly(ISolution soln)
        {
            
            var schedule = soln as Schedule;
            var drivers = schedule.GetDrivers();

            var retval = 0;
            foreach (Driver d in drivers)
            {
                var s = schedule.DriverSchedule(d);
                
                for (var shift = 1; shift < s.Length-2; shift += 2) // only night shifts
                {
                    if (s[shift] && s[shift + 1]) retval++;
                }
            }

            return retval;
        }

        public static double DayOffPrefs(ISolution soln)
        {
            var schedule = soln as Schedule;

            var drivers = schedule.GetDrivers();

            var retval = 0;
            foreach (Driver d in drivers)
            {
                var s = schedule.DriverSchedule(d);
                foreach (var pref in d.PrefDaysOff)
                {
                    if (s[pref * 2] || s[pref * 2 + 1])
                    {
                        // aww sorry dude
                    }
                    else retval++;
                }
            }

            return retval;
        }
        public static double LongRest(ISolution soln)
        {
            var schedule = soln as Schedule;

            var drivers = schedule.GetDrivers();
            var days = schedule.GetShifts().GetLength(0) / 2;

            var retval = 0;
            foreach (Driver d in drivers)
            {
                var consecDays = 0;
                for (int day = 0; day < days; day++)
                {
                    if (schedule.WorkingOn(d, day))
                    {
                        if (consecDays >= 5) retval++;
                        consecDays = 0;
                    }
                    else consecDays++;
                }
                if (consecDays >= 5) retval++;           
            }

            return retval;
        }
    }
}