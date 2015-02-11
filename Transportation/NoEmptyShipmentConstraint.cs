using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    // make sure all shipment records have a positive Tons amount
    public class NoEmptyShipmentConstraint : IConstraint
    {
        public bool CheckConstraint(ISolution soln)
        {
            var plan = soln as TransportationPlan;
            return plan.Trips.All(trip => trip.Shipments.All(shipment => shipment.Tons > 0));
        }
    }
}
