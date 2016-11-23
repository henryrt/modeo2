using RTH.Modeo2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.BusDrivers
{
    internal class ByLineCreate : IAlgorithm
    {
        static Random rand = new Random();

        public void Run(ISolver solver)
        {
           
            //create empty schedule
            var s = solver as BusSolver;
            var sched = new Schedule(s.Problem);
            s.AddSolution(sched);

            //populate each Line
            var numLines = sched.GetShifts().GetLength(1);
            for (int i = 0; i < numLines; i++)
            {
                var drivers = s.Problem.DriversForLine(i).ToArray();
                var d = rand.Next(drivers.Length);
                for (int index = 0; index<2*s.Problem.NumDays; index++)
                {
                    sched.SetShift(index, i, drivers[d]);
                    d++;
                    if (d >= drivers.Length) d = 0;
                }
            }
        }
    }
}
