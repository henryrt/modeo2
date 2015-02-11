using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public class AllOrdersShippedConstraint : IConstraint
    {
        // adds all tons shipped for each order and ensures the complete orderbook was shipped
        public bool CheckConstraint(ISolution soln)
        {
            var plan = soln as TransportationPlan;
            var orders = plan.Problem.Orders;
            var trips = plan.Trips;

            foreach(Order order in orders)
            {
                var target = order.Tons;
                var total = trips.Where(trip => (trip.Destination == order.Destination)).Sum(t => t.Shipments.Where(shipment => shipment.Order == order).Sum(s => s.Tons));
            }
            var ret = 
             orders.All(
                order => order.Tons == trips.Where(trip => (trip.Destination == order.Destination)).Sum(t => t.Shipments.Where(shipment => shipment.Order == order).Sum(s => s.Tons))
                );
            return ret;
        }
    }
}
