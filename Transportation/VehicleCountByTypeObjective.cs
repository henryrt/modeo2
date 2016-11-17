using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public class VehicleCountByTypeObjective : IObjective
    {
        public string Name
        {
            get; set;
        }
        public Type DataType { get { return typeof(int); } }
        public string Format { get; set; } = "D"; // base 10 integer

        public int Penalty(double val)
        {
            return 0; // display only, will not optimize
        }

        public int Penalty(ISolution soln)
        {
            return 0;
        }

        public double Value(ISolution soln)
        {
            return (soln as TransportationPlan).Trips.Where(t => t.Vehicle.Name == this.Name).Count();

        }
    }
}
