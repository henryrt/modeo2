using System;
using RTH.Modeo2;
using System.Linq;
using System.Collections.Generic;

namespace RTH.BusDrivers
{
    internal class ReduceExcessLates : IAlgorithm
    {
        private static Random rand = new Random();

        IObjective ObjExcessLates = null;
        
        public void Run(ISolver solver)
        {
            var s = solver as BusSolver;

            if (ObjExcessLates == null)
            {
                ObjExcessLates = s.DataStore.GetEnumerable<IObjective>().Where(o => o.Name == "ExcessLate").SingleOrDefault();
                if (ObjExcessLates == null) return;
            }
            // get a random solution and remove a shift from a driver with excess lates
            var schedule = s.DataStore.GetRandom<ISolution>() as Schedule;
            if (schedule == null) return;

            var eval = schedule.Evaluate(ObjExcessLates);
            if (eval.Value > 0)
            {
                var shifts = schedule.GetShifts();
                var lates = new Dictionary<Driver, int>();
                Objectives.Lates(shifts, lates);

                bool modified = false;
                //choose a random driver for each line
                for (int line = 0; line < s.Problem.NumLines; line++)
                { 
                    var driver = s.Problem.GetRandomDriverForLine(line);
                    if (driver != null && lates.Keys.Contains(driver))
                    {
                        var numLates = lates[driver];
                        if (numLates > 4)
                        {
                            // find a late shift and unassign it

                            var r = rand.Next(numLates);
                            for (int index = 1; index < shifts.GetLength(0); index += 2)
                            {
                                if (shifts[index, line] == driver)
                                {
                                    if (r == 0)
                                    {
                                        modified = modified || schedule.SetShift(index, line, null);
                                        // done with this driver and line
                                        goto driverLoop;
                                    }
                                    else
                                    {
                                        r--;
                                    }
                                }
                            }
                        }
                    }
                driverLoop:;
                }
                if (modified) schedule.Algorithm += "|Excess";
            }
        }
    }
}