using System;
using RTH.Modeo2;
using System.Linq;

namespace RTH.BusDrivers
{
    internal class RandomAssignments : IAlgorithm
    {
        private bool RespectDayOffPrefs = false;

        public RandomAssignments(bool RespectDayOffPrefs = false)
        {
            this.RespectDayOffPrefs = RespectDayOffPrefs;
        }

        public void Run(ISolver solver)
        {
            // get a random solution and assign a random driver to empty assignments
            var s = solver as BusSolver;
            var schedule = s.DataStore.GetRandom<ISolution>() as Schedule;
            if (schedule == null) return;
           
            foreach(var a in schedule.EmptyAssignments())
            {
                var driver = s.Problem.GetRandomDriverForLine(a.Line);
                var OK = true;

                if (RespectDayOffPrefs)
                {
                    OK = !driver.PrefDaysOff.Contains(a.Day);
                }
                if (OK && schedule.SetShift(a.Day, a.Shift, a.Line, driver))
                {
               
                }
               
            }
        }
    }
}