using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public interface IConstraint
    {
        // return true if solution meets constraint
        bool CheckConstraint(ISolution soln);
    }
}
