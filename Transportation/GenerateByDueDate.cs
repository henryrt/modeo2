using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public class GenerateByDueDate : IAlgorithm
    {
        string VehicleName;

        public GenerateByDueDate(string vehicleName)
        {
            VehicleName = vehicleName;
        }
        public void Run(ISolver solver)
        {
            // for each order find the cheapest mode that gets it there on time    
            var problem = (solver as TransportationSolver).Problem;

            var plan = new TransportationPlan(problem);        

            bool done = false;
            while (!done) {
                done = true;
                foreach (var order in problem.Orders)
                {
                   var tons = order.Tons;

                    // get rates for all modes that will arrive on time
                    var rates = order.Destination.Rates.Where(r => problem.StartDate.Add(r.Duration) <= order.DueDate);


                    VehicleType selectedMode = (rates.Count() != 0) ?   // if any found, use cheapest per ton (when filled to capacity)
                        rates.OrderBy(r => r.CostFunction(r.VehicleType.Capacity) / r.VehicleType.Capacity).First().VehicleType :
                        order.Destination.Rates.OrderBy(r => r.Duration).First().VehicleType;   // otherwise use fastest mode

                    // load 'em up
                    while (tons > 0)
                    {
                        var trip = new Trip()
                        {
                            DepartureDate = problem.StartDate,
                            Destination = order.Destination,
                            Vehicle = selectedMode,
                            Shipments = new List<Shipment>() { new Shipment() { Order = order, Tons = Math.Min(tons, selectedMode.Capacity) } }
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
