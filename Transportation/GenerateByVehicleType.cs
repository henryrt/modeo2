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

        public GenerateByVehicleType(string vehicleName)
        {
            VehicleName = vehicleName;
        }
        public void Run(ISolver solver)
        {
            // ship everything by Express carrier      
            var problem = (solver as TransportationSolver).Problem;

            var plan = new TransportationPlan(problem);

            var express = problem.VehicleTypes.Where(v => v.Name == VehicleName).First();

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
                            Vehicle = express,
                            Shipments = new List<Shipment>() { new Shipment() { Order = order, Tons = Math.Min(tons, express.Capacity) } }
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
