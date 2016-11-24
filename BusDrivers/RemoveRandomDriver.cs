using System;
using RTH.Modeo2;

namespace RTH.BusDrivers
{
    internal class RemoveRandomDriver : IAlgorithm
    {
        public void Run(ISolver solver)
        {
            // get a random solution and remove two drivers
            var s = solver as BusSolver;
            if (s.DataStore.Count<ISolution>() == 0) return;
            var schedule = s.DataStore.GetRandom<ISolution>() as Schedule;
            if (schedule == null) return;

            bool modified = false;

            //choose a random driver
            for (int i = 0; i < 2; i++)
            {
                var driver = s.Problem.GetRandomDriver();

                for (int line = 0; line < schedule.GetShifts().GetLength(1); line++)
                {
                    for (int index = 0; index < schedule.GetShifts().GetLength(0); index++)
                    {
                        if (schedule.GetShifts()[index, line] == driver)
                            modified = modified || schedule.SetShift(index, line, null);
                    }
                }
            }
            if (modified) schedule.Algorithm += "!RemDriver";
        }
    }
}