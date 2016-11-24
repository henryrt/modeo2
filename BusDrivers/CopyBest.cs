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
            var bestSoln = s.BestSchedule(Name);
            if (bestSoln != null)
                for (int i = 0; i < Copies; i++)
                {
                    s.DataStore.Add<ISolution>(bestSoln.Copy());
                }
        }
    }
}