using System;
using RTH.Modeo2;

namespace RTH.BusDrivers
{
    internal class RandomAssignments : IAlgorithm
    {
        public void Run(ISolver solver)
        {
            // get a random solution and assign a random driver to empty assignments
            var s = solver as BusSolver;
            var schedule = s.DataStore.GetRandom<ISolution>() as Schedule;
            if (schedule == null) return;
           
            foreach(var a in schedule.EmptyAssignments())
            {
                a.Driver = s.DataStore.GetRandom<Driver>();
                schedule.SetShift(a);
            }
        }
    }
}