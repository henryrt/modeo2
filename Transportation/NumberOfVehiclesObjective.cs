using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public class NumberOfVehiclesObjective : IObjective
    {
        public string Name
        {
            get
            {
                return "#Veh";
            }
        }
        public Type DataType { get { return typeof(int); } }
        public string Format { get; set; } = "D"; // base 10 integer

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
            return plan.Trips.Count();
        }
    }
}
