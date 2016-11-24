using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTH.Modeo2;

namespace RTH.BusDrivers
{
    public class BusSolver : BaseSolver
    {
        public ProblemStatement Problem;

        public BusSolver() : base() { }

        public BusSolver(int msec) : base(msec) { }

        internal Schedule NewSchedule(string algName = "")
        {
            return new Schedule(Problem.Drivers.ToArray(), Problem.NumDays, Problem.NumLines)
            {
                Algorithm = algName
            };
        }

        internal Schedule BestSchedule(string objectiveName)
        {
            // return the schedule with the lowest penalty for given objective
            var ds = DataStore;

            var schedules = DataStore.GetEnumerable<ISolution>().ToList();

            var obj = DataStore.GetEnumerable<IObjective>().Where(o => o.Name == objectiveName).SingleOrDefault();
            if (obj == null) return null;

            var q = (from sch in schedules orderby sch.Evaluate(obj).Penalty select sch);
            var bestSoln = q.FirstOrDefault();
            return bestSoln as Schedule;
        }
    }
}
