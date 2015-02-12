using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public class GenerateByVehicleType : IAlgorithm
    {
        string VehicleName;

        // ship everything by a single type of carrier      

        public GenerateByVehicleType(string vehicleName)
        {
            VehicleName = vehicleName;
        }
        public void Run(ISolver solver)
        {
            var problem = (solver as TransportationSolver).Problem;

            var plan = new TransportationPlan(problem);

            var vehicle = problem.VehicleTypes.Where(v => v.Name == VehicleName).First();

            bool done = false;
            while (!done) {
                done = true;
                foreach (var order in problem.Orders)
                {
                    var tons = order.Tons;

                    while(tons > 0) {
                        var trip = new Trip()
                        {
                            DepartureDate = problem.StartDate,
                            Destination = order.Destination,
                            Vehicle = vehicle,
                            Shipments = new List<Shipment>() { new Shipment() { Order = order, Tons = Math.Min(tons, vehicle.Capacity) } }
                        };
                        tons -= trip.Shipments[0].Tons;
                        plan.AddTrip(trip);
                    };
                }
            }
            solver.AddSolution(plan);
        }
    }
}
