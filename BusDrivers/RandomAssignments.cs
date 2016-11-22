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
                var driver = s.DataStore.GetRandom<Driver>();
                if (schedule.SetShift(a.Day, a.Shift, a.Line, driver))
                {
                //    Console.WriteLine("Assigned " + a);
                }
               // else Console.WriteLine("Not assigned " + a);

                //if (schedule.BookingViolation()) throw new ApplicationException("Invalid Schedule created.");
            }
        }
    }
}