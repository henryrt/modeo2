using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RTH.Modeo2;

namespace RTH.BusDrivers
{
    public class Calculations 
    {
        public double UnassignedShifts(BaseSolution soln)
        {
            var schedule = soln as Schedule;
            return schedule.GetAssignments().Where(a => a.Driver == null).Count() * 1.0;        
        }
    }
}
