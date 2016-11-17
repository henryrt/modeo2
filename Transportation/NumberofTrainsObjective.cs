using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public class NumberOfTrainsObjective
        : IObjective
    {
        public string Name { get { return "Trains"; } }
        public Type DataType { get { return typeof(int); } }
        public string Format { get; set; } = "D"; // base10 decimal


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
            var railcars = from t in plan.Trips
                           where t.Vehicle.Name == "Railcar"
                           select t;
            var cars = railcars.Count();

            var q = from t in railcars
                    where t.Vehicle.Name == "Railcar"
                    group t by new { t.Destination, t.DepartureDate }
                    into g
                    select new { Destination = g.Key.Destination,
                                 DepartureDate = g.Key.DepartureDate,
                                 Railcars = g.Count()
                    };

            var j = q.Count();
            return j;
                    
            //plan.Trips.Where(t => t.Vehicle.Name.Equals("Railcar")).GroupBy(t => t.Destination)
            //double cost = 0.0;
            //foreach (var trip in plan.Trips) cost += trip.Cost;
            //return cost;
        }
    }
}
