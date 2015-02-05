using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public delegate bool Filter(CollectionManager cm, ISolution soln);

    public class BaseSolver : ISolver
    {
        #region instance variables
        public CollectionManager DataStore { get; } = new CollectionManager();
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

        #region Methods
        public void ApplyFilter(Filter f)
        {
            foreach(var soln in DataStore.GetEnumerable<ISolution>())
            {
                soln.Filtered = soln.Filtered || f(DataStore, soln);
            }
        }

        public void ApplyFilters()
        {
            foreach (var f in DataStore.GetEnumerable<Filter>()) ApplyFilter(f);
        }

        public void RemoveFilteredSolutions()
        {
            DataStore.RemoveAll<ISolution>(new Predicate<ISolution>(s => s.Filtered));
        }

        public void RemoveDominatedSolutions()
        {
            var solns = DataStore.GetEnumerable<ISolution>();
            var objs = DataStore.GetEnumerable<IObjective>();
            DataStore.RemoveAll<ISolution>(new Predicate<ISolution>(s => s.IsDominated(solns, objs)));
        }
        #endregion
    
    }
}
