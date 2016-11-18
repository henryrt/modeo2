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
        public string Name { get; }
        public Type DataType { get { return typeof(double); } }
        public string Format { get; set; } // = "999999.##";  // C# 6

        private readonly double target;
        private readonly double underPenaltyFactor;
        private readonly double overPenaltyFactor;

        public ValueProvider ValueProvider;
     
                    
        public TargetObjective(string Name, double target, double underPenaltyFactor, double overPenaltyFactor)
        {
            this.target = target;
            this.underPenaltyFactor = underPenaltyFactor;
            this.overPenaltyFactor = overPenaltyFactor;
            this.Name = Name;
        }
        public TargetObjective(string Name, double target) : this(Name, target, 1, 1)
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
