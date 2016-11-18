using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    class Program
    {
        static void Main(string[] args)
        {
            // new TransportationStudy().Run(args);

            var ps = BusDrivers.ProblemStatement.CreateProblem();
          /*
            BaseSolver solver = new BaseSolver(new TimedCollectionManager(new CollectionManager()), 250);
            var store = solver.DataStore;

            var objs = new List<IObjective>()
            {
                new TargetObjective("Targ 4", 4) { ValueProvider = (s) => { return ((Solution1)s).N; } },
                new TargetObjective("Targ 9", 9) { ValueProvider = (s) => { return ((Solution1)s).N; } }
            };

            store.AddCollection<IObjective>(objs);
            store.Add<IAlgorithm>(new CreateAlgorithm());
            store.Add<IStopCondition>(new PopulationLimitCondition(1000));
            
            solver.Start();

            Console.WriteLine("#Solutions = " + store.Count<ISolution>());
            
            ShowSolutions(store.GetEnumerable<ISolution>(), objs);
            
            //store.Add<Filter>(Filter1);
            //store.Add<Filter>(Filter2);

            //solver.RemoveFilteredSolutions(true);
            //Console.WriteLine("#Solutions = " + store.Count<ISolution>());

            //var solns = store.GetReadOnlyCollection<ISolution>();
            //foreach (Solution1 s in solns)
            //{
            //    var dominated = s.IsDominated(solns, objs);
            //    var evals = s.Evaluate(objs);
            //    foreach (var eval in evals.Values)
            //    {
            //        Console.Write(eval.Penalty + " ");
            //    }
                
            //    Console.WriteLine(String.Format(dominated +" "+ s.N));
            //    store.Add<ISolution>(new Solution1());
            //}

            //Console.WriteLine("#Solutions = " + store.Count<ISolution>());
            //ShowSolutions(store.GetEnumerable<ISolution>(), objs);

            solver.RemoveDominatedSolutions();
            Console.WriteLine("#Solutions = " + store.Count<ISolution>());
            ShowSolutions(store.GetEnumerable<ISolution>(), objs);


            //store.Add<IConstraint>(new MustBeLessThanSixConstraint());
            //solver.ApplyConstraints();
            //Console.WriteLine("#Solutions = " + store.Count<ISolution>());
            //ShowSolutions(store.GetEnumerable<ISolution>(), objs);

            //for (int i=0; i<10; i++)
            //{
            //    var s = store.GetRandom<ISolution>() as Solution1;
            //    ShowPenalties(s.Evaluate(objs).Values);
            //    Console.WriteLine(s.N);
            //}
            //Console.WriteLine((store as TimedCollectionManager).Log);

            var grid = solver.getGrid();
            foreach (string[] row in grid)
            {
                for (var ix = 0; ix < row.Length; ix++) Console.Write(row[ix]+'\t');
                Console.WriteLine();
            }
            */
        }

        //private static void ShowSolutions(IEnumerable<ISolution> solns, IEnumerable<IObjective> objs)
        //{
        //    foreach (Solution1 s in solns)
        //    {
        //        var dominated = s.IsDominated(solns, objs);
        //        var evals = s.Evaluate(objs);
        //        ShowPenalties(evals.Values);
        //        Console.WriteLine(String.Format(" N = {0} {1}", s.N, (dominated? "" : "*")));
        //    }
        //}
        private static void ShowPenalties(IEnumerable<Evaluation> evals)
        {
            Console.Write("[ ");
            foreach (var eval in evals)
            {
                Console.Write(eval.Penalty + " ");
            }
            Console.Write("]");

        }

        public static bool Filter1(ICollectionManager cm, ISolution soln)
        {
            // flag any solutions that have an undesired objective penalty
            var objs = cm.GetEnumerable<IObjective>();
            return objs.Any(obj => soln.Evaluate(obj).Penalty > 11);
        }
        public static bool Filter2(ICollectionManager cm, ISolution soln)
        {
            // flag any solutions that have the first objective value < 3
            var objs = cm.GetEnumerable<IObjective>();
            return objs.First().Value(soln) < 3;
        }

        private static TimeSpan Timer(Action a)
        {
            var startTime = DateTime.Now;
            a();
            return DateTime.Now.Subtract(startTime);
        }
    }
    /*

    class Objective1 : IObjective
    {
        public string Name {  get { return "Objective1"; } }
        public double Value(ISolution soln)
        {
            return Math.Abs((soln as Solution1).N - 5);
        }

        public int Penalty(ISolution soln)
        {
            return Penalty(Value(soln));
        }

        public int Penalty(double val)
        {
            return (int)val;
        }
    }
    class Objective2 : IObjective
    {
        public string Name { get { return "Objective2"; } }

        public double Value(ISolution soln)
        {
            return Math.Abs((soln as Solution1).N - 8);
        }

        public int Penalty(ISolution soln)
        {
            return Penalty(Value(soln));
        }

        public int Penalty(double val)
        {
            return (int)val;
        }
    }
    class Solution1 : BaseSolution
    {
        private static Random rand = new Random();
        public int N = rand.Next(10);

        public override bool IsDuplicate(ISolution soln)
        {
            bool ret = (this.N == ((Solution1)soln).N);
            return ret;
        }
        public override bool Equals(object obj)
        {
            return this.N == ((Solution1)obj).N;
        }

        public override int GetHashCode()
        {
            return N;
        }
    }

    class CreateAlgorithm : IAlgorithm
    {
       
        public void Run(ISolver solver)
        {
            // create a new solution and add it
            ISolution s = new Solution1();
            solver.AddSolution(s);
        }

 
    }

    class MustBeLessThanSixConstraint : IConstraint
    {
        public bool CheckConstraint(ISolution soln)
        {
            return ((Solution1)soln).N < 6;
        }
    }

    */
}
