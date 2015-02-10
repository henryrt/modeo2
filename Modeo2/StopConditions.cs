using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public class PopulationLimitCondition : IStopCondition
    {
        private int populationLimit;

        public PopulationLimitCondition(int n)
        {
            populationLimit = n;
        }

        public void Initialize()
        {
        }

        public bool ShouldStop(ICollectionManager cm)
        {
            return (cm.Count<ISolution>() >= populationLimit);
        }
    }

    public class TimerStopCondition : IStopCondition
    {
        int mSec = 0;
        DateTime stopTime;

        public TimerStopCondition(int msec)
        {
            mSec = msec;
        }
        public void Initialize()
        {
            stopTime = DateTime.Now.AddMilliseconds(mSec);
        }

        public bool ShouldStop(ICollectionManager cm)
        {
            return DateTime.Now > stopTime;
        }
    }

}
