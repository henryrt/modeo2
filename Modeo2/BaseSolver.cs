using System;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    // a Filter is a method that determines if a solution should be filtered out
    public delegate bool Filter(ICollectionManager cm, ISolution soln);

    public class BaseSolver : ISolver
    {
        #region instance variables
        public ICollectionManager DataStore { get; } 
        #endregion

        #region constructors

        public BaseSolver() : this(1000)  {  }

        public BaseSolver(int defaultTimeout)
        {
            DataStore = new CollectionManager();
            DataStore.Add<IStopCondition>(new TimerStopCondition(defaultTimeout));
        }

        public BaseSolver(ICollectionManager cm) : this(cm, 1000) { }

        public BaseSolver(ICollectionManager cm, int defaultTimeout)
        {
            DataStore = cm;
            DataStore.Add<IStopCondition>(new TimerStopCondition(defaultTimeout));
        }

        #endregion


        #region Initialization

        public void Start(int iterations)
        {
            // Initialize
            InitializeAll<IStopCondition>();

            var stop = false;
            var counter = 0;
            while (!stop)
            {
                // loop through each algorithm then test conditions to stop
                foreach (var algorithm in DataStore.GetEnumerable<IAlgorithm>())
                {
                    algorithm.Run(this);
                    if (CheckStopConditions())
                    {
                        stop = true;
                        break;
                    }
                }
                if (counter++ == iterations) return;
            }
        }
        public void Start() { Start(int.MaxValue); }

        public void InitializeAll<T>()
        {
            // check that T has an Initialize method
            var method = typeof(T).GetMethod("Initialize");
            if (method != null) foreach (var item in DataStore.GetEnumerable<T>()) method.Invoke(item, null);
        }
        #endregion

        #region Stopping
        public bool CheckStopConditions()
        {
            return DataStore.Any<IStopCondition>(sc => sc.ShouldStop(DataStore));
        }
        #endregion

        #region Clear methods
        public void ClearStopConditions()
        {
            DataStore.RemoveAll<IStopCondition>();
        }
        public void ClearFilters()
        {
            DataStore.RemoveAll<Filter>();
            foreach (var soln in DataStore.GetEnumerable<ISolution>()) soln.Filtered = false;
        }
        public void ClearConstraints()
        {
            DataStore.RemoveAll<IConstraint>();
        }
        #endregion

        #region Filters and Constraints
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


        public bool CheckConstraints(ISolution soln)
        {
            return DataStore.All<IConstraint>(c => c.CheckConstraint(soln));
        }

        public void ApplyConstraints()
        {
            DataStore.RemoveAll<ISolution>(soln => !CheckConstraints(soln));
        }
        #endregion

        #region Add and Remove Solutions
        public void RemoveFilteredSolutions(bool applyFiltersFirst = false)
        {
            if (applyFiltersFirst) ApplyFilters();
            DataStore.RemoveAll<ISolution>(new Predicate<ISolution>(s => s.Filtered));
        }

        public void RemoveDominatedSolutions()
        {
            var solns = DataStore.GetEnumerable<ISolution>();
            var objs = DataStore.GetEnumerable<IObjective>();
            DataStore.RemoveAll<ISolution>(new Predicate<ISolution>(s => s.IsDominated(solns, objs)));
        }

        public bool AddSolution(ISolution soln)
        {
            if (!CheckConstraints(soln)) return false;
            soln.Evaluate(DataStore.GetEnumerable<IObjective>());
            DataStore.Add<ISolution>(soln);
            return true;
        }

        #endregion

        public ArrayList getGrid()
        {
            return getGrid(DataStore.GetEnumerable<ISolution>());
        }

        public ArrayList getGrid(IEnumerable<ISolution> results)
        {
            var al = new ArrayList(results.Count() + 1);

            var objs = DataStore.GetEnumerable<IObjective>();

            var ar = new string[objs.Count()];
            //header row
            var i = 0;
            foreach (var obj in objs) ar[i++] = obj.Name;
            al.Add(ar);
            foreach (var soln in results)
            {
                ar = new string[objs.Count()];
                i = 0;
                foreach (var obj in objs)
                {
                    var eval = soln.Evaluate(obj);
                    //ar[i++] = eval.Value.ToString() + " (" + eval.Penalty.ToString() + ")";
                    ar[i++] = eval.Value.ToString();
                }
                al.Add(ar);
            }

            return al;
        }

    }
}
