using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public class LateOrdersObjective : IObjective
    {
        public string Name
        {
            get
            {
                return "#Late";
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
            var orders = plan.Problem.Orders;
            var trips = plan.Trips;

            // how many orders have trips that arrive after the due date?
            // only examine trips to the order destination that have shipments for the order
            var q = orders.Where(order =>
                trips.Where(trip => (trip.Destination == order.Destination) && (trip.ArrivalDate > order.DueDate) && (trip.Shipments.Exists(shipment => shipment.Order == order))).Any()
                );

            return q.Count();
        }
    }
}
