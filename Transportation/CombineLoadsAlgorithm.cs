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
            var plan = (solver as TransportationSolver).DataStore.GetRandom<ISolution>() as TransportationPlan;

            Option2(solver as TransportationSolver, plan);

        }

        private static void Option2(TransportationSolver solver, TransportationPlan plan)
        {
            // for each destination, start with the emptiest vehicle and combine it with the next emptiest vehicle 
            // use the smallest vehicle that will hold the combination and set departure to the earlier of the two trips

            var tripsByDestination = plan.Trips.GroupBy(t => t.Destination).Select(g => g).ToList();

            tripsByDestination.ForEach(dest =>
            {
                if (dest.Count() >= 2)
                {
                    var trips = dest.OrderBy(trip => trip.Tons).ToList();
                    var trip1 = trips.ElementAt(0);
                    var trip2 = trips.ElementAt(1);
                    var departure = (trip1.DepartureDate < trip2.DepartureDate) ? trip1.DepartureDate : trip2.DepartureDate;
                    var vehicle = plan.Problem.VehicleTypes.Where(v => v.Capacity >= trip1.Tons + trip2.Tons).OrderBy(v => v.Capacity).FirstOrDefault();
                    if (vehicle != null)
                    {
                        CreateNewPlan(solver, plan, trip1, trip2, vehicle, departure);
                    }
                }
            });
        }
        
        private static void Option1(TransportationSolver solver, TransportationPlan plan)
        {
            // find 2 underfilled loads of same vehicletype with same departure and destination and combine them
            // make sure the combined load fits in one vehicle

            // try ten times to find a pair of trips
            for (int attempts = 0; attempts < 10; attempts++)
            {
                // pick two random vehicles
                var i1 = rand.Next(plan.Trips.Count());
                var i2 = rand.Next(plan.Trips.Count());

                if (i1 != i2)
                {
                    var trip1 = plan.Trips.ElementAt(i1);
                    var trip2 = plan.Trips.ElementAt(i2);

                    if ((trip1.DepartureDate == trip2.DepartureDate) &&
                         (trip1.Destination == trip2.Destination) &&
                         (trip1.Vehicle == trip2.Vehicle) &&
                         (trip1.Tons + trip2.Tons < trip1.Vehicle.Capacity))
                    {
                        // create new plan and copy old plan                        // do not modify plans in-place
                        CreateNewPlan(solver, plan, trip1, trip2, trip1.Vehicle, trip1.DepartureDate);

                        //Console.WriteLine(this + " success");
                        break; // exit loop
                    }
                }
            }
        }

        private static void CreateNewPlan(TransportationSolver solver, TransportationPlan plan, Trip trip1, Trip trip2, VehicleType vehicle, DateTime departure)
        {
            var newPlan = new TransportationPlan(plan.Problem);

            // Trip records can be used by more than one Plan
            // Do not modify them
            foreach (var trip in plan.Trips)
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
                DepartureDate = departure,
                Vehicle = vehicle,
                Shipments = new List<Shipment>()
            };

            foreach (var item in dict)
            {
                var s = new Shipment() { Order = item.Key, Tons = item.Value };
                newTrip.Shipments.Add(s);
            }
            newPlan.AddTrip(newTrip);
            if (!solver.AddSolution(newPlan))
            {
                Console.WriteLine("ERROR: New Solution is duplicate or failed constraint.");
            }
        }
    }
}
