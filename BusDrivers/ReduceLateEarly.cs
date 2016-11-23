using System;
using RTH.Modeo2;

namespace RTH.BusDrivers
{
    internal class ReduceLateEarly : IAlgorithm
    {
        public void Run(ISolver solver)
        {
            // get a random solution and swap drivers when a late-early occurs
            var s = solver as BusSolver;
            var schedule = s.DataStore.GetRandom<ISolution>() as Schedule;
            if (schedule == null) return;

            //choose a random driver
            var driver = s.Problem.GetRandomDriver();
            //look at schedule
            var ds = schedule.DriverSchedule(driver);
            //find a late-early
            for (int index = 1; index < ds.Length-1; index += 2)
            {
                if (ds[index] && ds[index+1])
                {
                    //late followed by early
                    var shifts = schedule.GetShifts();
                    // find line
                    for (int line=0; line<shifts.GetLength(1); line++)
                    {
                        if (driver.Equals(shifts[index,line]))
                        {
                            //swap late shift with early shift driver
                            var earlyDriver = shifts[index - 1, line];
                            schedule.SetShift(index - 1, line, null);
                            schedule.SetShift(index, line, earlyDriver);
                            schedule.SetShift(index - 1, line, driver);

                            return;
                        }
                    }
                }
            }
            
        }
    }
}