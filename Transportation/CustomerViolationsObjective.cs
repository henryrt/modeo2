using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public class CustomerViolationsObjective : IObjective
    {
        // how many vehicles are late (for special customers only)
        public string Name
        {
            get
            {
                return "Violation";
            }
        }

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
            var trips = plan.Trips.Where(t => t.Shipments.Exists(s => s.Order.Customer.Special && s.Order.DueDate < t.ArrivalDate));
            return trips.Count();
        }
    }
}
