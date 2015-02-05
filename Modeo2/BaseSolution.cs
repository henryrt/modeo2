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

        public bool Dominates(ISolution soln, IEnumerable<IObjective> objs)
        {
            // Can not dominate itself
            if (this.Equals(soln)) return false;

            // get Evaluations for each solution
            var thisEval = Evaluate(objs);
            var thatEval = soln.Evaluate(objs);

            //This dominates if ALL its Penalties are less than or equal to That.
            return objs.All( obj => thisEval[obj].Penalty <= thatEval[obj].Penalty );
            
        }

        public Dictionary<IObjective, Evaluation> Evaluate(IEnumerable<IObjective> objs)
        {
            // if the evaluations are not cached, compute them and cache them
            return (Evaluations ?? (Evaluations = ComputeEvaluations(objs)));
        }

        private Dictionary<IObjective, Evaluation> ComputeEvaluations(IEnumerable<IObjective>  objs)
        {
            var e = new Dictionary<IObjective, Evaluation>();
            var list = objs.ToList<IObjective>(); // performance?
            

            //Evaluate the solution for each objective
            list.ForEach(obj => e[obj] = Evaluate(obj));

            return e;
        }

        public Evaluation Evaluate(IObjective obj)
        { 
            var eval = new Evaluation();
            eval.Value = obj.Evaluate(this);                 
            eval.Penalty = obj.Penalty(eval.Value);
            return eval;
        }

        public bool IsDominated(IEnumerable<ISolution> solns, IEnumerable<IObjective> objs)
        {
            return solns.Any(s => s.Dominates(this, objs));
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
    }
}
