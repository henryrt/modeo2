using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public class CombineLoadsAlgorithm : IAlgorithm
    {
        static Random rand = new Random();

        public void Run(ISolver solver)
        {
            // select a solution at random
            // find 2 underfilled loads of same vehicletype with same departure and destination and combine them
            // make sure the combined load fits in one vehicle

            var plan = (solver as TransportationSolver).DataStore.GetRandom<ISolution>() as TransportationPlan;

            // pick two random vehicles
            var i1 = rand.Next(plan.Trips.Count());
            var i2 = rand.Next(plan.Trips.Count());

            if (i1 != i2)
            {
                var trip1 = plan.Trips.ElementAt(i1);
                var trip2 = plan.Trips.ElementAt(i2);

                if ( (trip1.DepartureDate == trip2.DepartureDate)  &&
                     (trip1.Destination == trip2.Destination)  &&
                     (trip1.Tons + trip2.Tons < trip1.Vehicle.Capacity))
                {
                    // create new plan and copy old plan
                    // do not modify plans in-place
                    var newPlan = new TransportationPlan(plan.Problem);

                    // Trip records can be used by more than one Plan
                    // Do not modify them
                    foreach(var trip in plan.Trips)
                    {
                        if ((trip != trip1) && (trip != trip2)) newPlan.AddTrip(trip);
                    }
                    var totalTons = trip1.Tons + trip2.Tons;

                    //collapse shipments by order
                    var shipments = new List<Shipment>(trip1.Shipments);
                    shipments.AddRange(trip2.Shipments);

                    var dict = shipments.GroupBy(s => s.Order).ToDictionary(g => g.Key, g => g.Sum(s => s.Tons));

                    var newTrip = new Trip()
                    {
                        Destination = trip1.Destination,
                        DepartureDate = trip1.DepartureDate,
                        Vehicle = trip1.Vehicle,
                        Shipments = new List<Shipment>()
                    };

                    foreach (var item in dict)
                    {
                        var s = new Shipment() { Order = item.Key, Tons = item.Value };
                        newTrip.Shipments.Add(s);                             
                    }
                    newPlan.AddTrip(newTrip);
                    solver.AddSolution(newPlan);
                }
            }

        }
    }
}
