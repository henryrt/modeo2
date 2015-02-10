using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public interface ISolver
    {
        // Will check constraints, evaluate solution and add to population
        bool AddSolution(ISolution soln);

        // Removes any solutions that do not meet constraints
        void ApplyConstraints();
    }
}
