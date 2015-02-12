using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public class TransportationSolver : BaseSolver
    {
        public ProblemStatement Problem;

        public TransportationSolver() : base() { }

        public TransportationSolver(int msec) : base(msec) { }
    }
}
