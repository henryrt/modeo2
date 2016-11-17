using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public class ObjectiveFilter
    {
        public string ObjectiveName { get; set; }
        public double LimitValue { get; set; }

        public bool GetFilter(ICollectionManager cm, ISolution soln)
        {
            var obj = cm.GetEnumerable<IObjective>().Where(o => o.Name == ObjectiveName).First();
            return soln.Evaluate(obj).Value > LimitValue;
        }
    }
}
