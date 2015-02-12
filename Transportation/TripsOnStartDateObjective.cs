using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public class TripsOnStartDateObjective : IObjective
    {
        public string Name
        {
            get
            {
                return "StartDateVeh";
            }
        }

        public int Penalty(double val)
        {

            // no penalty for less than or equal to 10
            return Math.Max(0, Convert.ToInt32(val) - 10);
        }

        public int Penalty(ISolution soln)
        {
            return Penalty(Value(soln));
        }

        public double Value(ISolution soln)
        {
            var plan = soln as TransportationPlan;
            var start = plan.Problem.StartDate;

            return plan.Trips.Where(trip => trip.DepartureDate == start).Count();
        }
    }
}
