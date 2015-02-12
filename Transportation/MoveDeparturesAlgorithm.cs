using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public class MoveDeparturesAlgorithm : IAlgorithm
    {
        // pick a random plan and a number of random trips and move the departure forward 8, 16 or 24 hours.
        private static Random rand = new Random();
        public void Run(ISolver solver)
        {

            var plan = (solver as TransportationSolver).DataStore.GetRandom<ISolution>() as TransportationPlan;
            var newPlan = new TransportationPlan(plan.Problem);

            var nRandomTrips = rand.Next(5) + 1;  // 1 to 5 trips
            var tripsToBeModified = new HashSet<Trip>(); // use a hashset as it will not allow duplicates to be added
            for(var i = 0; i < nRandomTrips; i++)
            {
                tripsToBeModified.Add(plan.Trips.ElementAt(rand.Next(plan.Trips.Count())));
            }

            // put all unmodified trips into new plan
            foreach (var trip in plan.Trips.Where(t => !tripsToBeModified.Contains(t)))
            {
                newPlan.AddTrip(trip);
            }

            foreach(var trip in tripsToBeModified)
            {
                var newDepartureDate = trip.DepartureDate.AddHours(8 * (rand.Next(3) + 1));
                newPlan.AddTrip(trip.MoveDepartureDate(newDepartureDate));
            }

            solver.AddSolution(newPlan);
        }
    }
}
