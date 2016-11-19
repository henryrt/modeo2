using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTH.Modeo2;

namespace RTH.BusDrivers
{
    public class BusSolver : BaseSolver
    {
        public ProblemStatement Problem;

        public BusSolver() : base() { }

        public BusSolver(int msec) : base(msec) { }

        internal Schedule NewSchedule()
        {
            return new Schedule(Problem.Drivers.ToArray(), Problem.NumDays, Problem.NumLines);
        }
    }
}
