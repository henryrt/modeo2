using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public class BaseSolver : ISolver
    {
        #region instance variables
        public CollectionManager Collections { get; } = new CollectionManager();
        #endregion

        #region constructors
        public BaseSolver()
        {
        }
        #endregion

        #region initialization methods
        #endregion

        #region Properties
        #endregion


    }
}
