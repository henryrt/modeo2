using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public class Evaluation
    {
        public double Value;
        public int Penalty;

        public bool IdenticalPenalty(Evaluation that)
        {
            return this.Penalty == that.Penalty;
        }

        // this Evaluation is worse than that one.
        public bool Worse(Evaluation that)
        {
            return this.Penalty > that.Penalty;
        }
    }
}
