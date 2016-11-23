using System;
using RTH.Modeo2;
using System.Linq;

namespace RTH.BusDrivers
{
    internal class CopyBest : IAlgorithm
    {
        private int Copies;
        private string Name;
        public CopyBest(int copies, string name)
        {
            Copies = copies;
            Name = name;
        }
        public void Run(ISolver solver)
        {
            var s = solver as BusSolver;
            var schedules = s.DataStore.GetEnumerable<ISolution>().Cast<Schedule>();

            var obj = s.DataStore.GetEnumerable<IObjective>().Where(o => o.Name == Name).SingleOrDefault();
            if (obj == null) return;


            var best = 99999;
            Schedule bestSoln = null;
            foreach (Schedule sched in schedules)
            {
                if (obj.Value(sched) < best)
                {
                    best = (int) obj.Value(sched);
                    bestSoln = sched;
                }
            }

            // make copies of best schedule
            if (bestSoln != null) for (int i=0; i<Copies; i++)
            {
                s.DataStore.Add<ISolution>(bestSoln.Copy());
            }
        }
    }
}