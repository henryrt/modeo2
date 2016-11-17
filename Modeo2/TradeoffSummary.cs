using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public class TradeoffSummary
    {
        public IObjective Objective1 { set; get; }
        public IObjective Objective2 { set; get; }
        public double Objective1Value { set; get; }
        public double Objective2Value { set; get; }
        public int Count { set; get; }
    }
}
