using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public class CostObjective : IObjective
    {
        public string Name { get { return "Cost"; } }
        public int Scale;

        public int Penalty(double val)
        {
            return Convert.ToInt32(val);
        }

        public int Penalty(ISolution soln)
        {
            return Penalty(Value(soln));
        }

        public double Value(ISolution soln)
        {
            var plan = soln as TransportationPlan;
            double cost = 0.0;
            foreach (var trip in plan.Trips) cost += trip.Cost;
            return cost;
        }
    }
}
