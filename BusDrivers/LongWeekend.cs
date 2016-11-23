using System;
using RTH.Modeo2;

namespace RTH.BusDrivers
{
    internal class LongWeekend : IAlgorithm
    {
        public void Run(ISolver solver)
        {
            var s = solver as BusSolver;

            // get a random solution and remove a shift from a driver before or after his days off
            var schedule = s.DataStore.GetRandom<ISolution>() as Schedule;
            if (schedule == null) return;

            var driver = s.Problem.GetRandomDriver();
            var ds = schedule.DriverSchedule(driver);
            for (int day = 0; day < s.Problem.NumDays; day++)
            {
                if (driver.DaysOff[day])
                {
                    Unassign(schedule, driver, day - 1);
                    Unassign(schedule, driver, day + 1);
                }
            }

        }

        private void Unassign(Schedule schedule, Driver driver, int day)
        {
            var shifts = schedule.GetShifts();
            if (day < 0 || day >= shifts.GetLength(0) / 2) return;
            if (schedule.WorkingOn(driver, day))
            {
                // find the shift and line being worked
                for (int line = 0; line < shifts.GetLength(1); line++)
                {
                    if (shifts[day * 2, line] == driver)
                    {
                        schedule.SetShift(day, 0, line, null);
                        return;
                    }
                    if (shifts[day * 2+1, line] == driver)
                    {
                        schedule.SetShift(day, 1, line, null);
                        return;
                    }
                }
            }
        }
    }
}