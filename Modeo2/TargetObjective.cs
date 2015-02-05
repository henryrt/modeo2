using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    // a function that takes a solution and computes the value for the objective
    public delegate double ValueProvider(ISolution soln);

    // The Objective takes a Target value and and Under Penalty and Over Penalty factor
    public class TargetObjective : IObjective
    {
        private readonly double target;
        private readonly double underPenaltyFactor;
        private readonly double overPenaltyFactor;

        public ValueProvider ValueProvider;
                 
        public TargetObjective(double target, double underPenaltyFactor, double overPenaltyFactor)
        {
            this.target = target;
            this.underPenaltyFactor = underPenaltyFactor;
            this.overPenaltyFactor = overPenaltyFactor;
        }
        public TargetObjective(double target) : this(target, 1, 1)
        {
        }
        public virtual double Value(ISolution soln)
        {
            return ValueProvider(soln);
        }

        public int Penalty(double val)
        {
            var delta = val - target;
            return delta > 0 ? (int)(overPenaltyFactor * delta) : (int)(-underPenaltyFactor * delta);
        }

        public int Penalty(ISolution soln)
        {
            return Penalty(Value(soln));

        }
    }
}
