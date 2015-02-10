using System;
using System.Collections;
using System.Collections.Generic;

namespace RTH.Modeo2
{
    public interface ISolution
    {
        // returns the Evaluation (Value and Penalty) of the Solution for the specified Objective
        Evaluation Evaluate(IObjective obj);

        // returns all the Evalautions of the Solution for the given Objectives
        Dictionary<IObjective, Evaluation> Evaluate(IEnumerable<IObjective> objs);

        // returns true if this Solution dominates the given Solution. This means that this solution 
        // has Penalties for all given Objectives that are less than or equal to the Penalties of the 
        // given Solution.
        bool Dominates(ISolution soln, IEnumerable<IObjective> objs);

        // returns true if the solution is dominated by any of the given Solutions
        bool IsDominated(IEnumerable<ISolution> solns, IEnumerable<IObjective> objs);

        bool Filtered { set; get; }

        // return true if all constraints are met
        bool CheckConstraints(ICollectionManager store);

    }
}