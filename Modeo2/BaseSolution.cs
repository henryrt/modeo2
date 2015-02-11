using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public class BaseSolution : ISolution
    {
        // Evaluations store a penalty value. Lower penalties are better.
        private Dictionary<IObjective, Evaluation> Evaluations;

        public bool Filtered
        {
            get;
            set;
        }

        public bool Dominated
        {
            get; set;
        }

        public void ClearEvaluationCache()
        {
            Evaluations = null;
            Filtered = false;
            Dominated = false;
        }

        public bool Dominates(ISolution soln, ICollectionManager store)
        {
            return Dominates(soln, store.GetEnumerable<IObjective>());
        }
        public bool Dominates(ISolution soln, IEnumerable<IObjective> objs)
        {
            // Can not dominate itself
            if (this == soln) return false;

            // if a different solution is Equal, then it is dominated, i.e. a duplicate
            if (this.Equals(soln)) return true;

            // get Evaluations for each solution
            var thisEvaluationSet = Evaluate(objs);
            var thatEvaluationSet = soln.Evaluate(objs);

            //foreach (var obj in objs) Console.WriteLine(String.Format("{0}\t{1}", thisEval[obj].Penalty, thatEval[obj].Penalty));

            // do not dominate another solution with identical penalties
            if (objs.All(obj => thisEvaluationSet[obj].IdenticalPenalty(thatEvaluationSet[obj]))) return false;

            //This dominates if That is worse on all objectives
            return objs.All(obj => thatEvaluationSet[obj].WorseOrEqual(thisEvaluationSet[obj]));

            
        }

        public Dictionary<IObjective, Evaluation> Evaluate(IEnumerable<IObjective> objs)
        {
            // if the evaluations are not cached, compute them and cache them
            return (Evaluations ?? (Evaluations = ComputeEvaluations(objs)));
        }

        public Dictionary<IObjective, Evaluation> Evaluate(ICollectionManager store)
        {
            return (Evaluations ?? (Evaluations = ComputeEvaluations(store.GetEnumerable<IObjective>())));
        }
        public Dictionary<IObjective, Evaluation> ComputeEvaluations(IEnumerable<IObjective> objs)
        {
            var e = new Dictionary<IObjective, Evaluation>();
            foreach (var obj in objs) e[obj] = Evaluate(obj);
            return e;
        }

        public Evaluation Evaluate(IObjective obj)
        {
            var eval = new Evaluation();
            eval.Value = obj.Value(this);
            eval.Penalty = obj.Penalty(eval.Value);
            return eval;
        }

        public bool IsDominated(IEnumerable<ISolution> solns, IEnumerable<IObjective> objs)
        {
            // no need to test a solution that is already dominated
            Dominated = solns.Where(s => !s.Dominated).Any(s => s.Dominates(this, objs));
            return Dominated;
        }

        /// <summary>
        /// Override with code to detect duplicate solutions
        /// </summary>
        /// <param name="soln"></param>
        /// <returns></returns>
        virtual public bool IsDuplicate(ISolution soln)
        {
            return false;
        }
        public bool IsDuplicate(IEnumerable<ISolution> solns)
        {
            return solns.Any(s => IsDuplicate(s));
        }

        public bool CheckConstraints(ICollectionManager store)
        {
            return store.All<IConstraint>(c => c.CheckConstraint(this));
        }
    }
}
