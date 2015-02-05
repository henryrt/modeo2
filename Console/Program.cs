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
            BaseSolver solver = new BaseSolver();
            var objectives = new List<IObjective>() {
                new TargetObjective(5) {
                    ValueProvider = (soln) => { return ((Solution1)soln).N; }
                },
                new TargetObjective(8) {
                    ValueProvider = (soln) => { return ((Solution1)soln).N; }
                }
                };
                var solutions = new List<ISolution>();
            for (int i = 0; i< 100; i++)
            {
                var s = new Solution1();
                if (!s.IsDuplicate(solutions))
                {
                    solutions.Add(s);
                }
            }

            var c = solver.Collections;
            c.AddCollection<List<IObjective>, IObjective>(objectives);
            c.AddCollection<List<ISolution>, ISolution>(solutions);

            Console.WriteLine(c.GetReadOnlyCollection<IObjective>());
            Console.WriteLine(c.GetReadOnlyCollection<ISolution>());

            var o1 = c.GetReadOnlyCollection<IObjective>();
            //o1.Add(new Objective1());
            foreach (var o2 in c.GetEnumerable<IObjective>())
            {
                Console.WriteLine(o2);
            }

            var objs = c.GetEnumerable<IObjective>();

            var solns = c.GetEnumerable<ISolution>();
            foreach(Solution1 s in solns)
            {
                var dominated = s.IsDominated(solns, objs);
                Console.WriteLine(String.Format("{0} {1} {2} {3}", s.Evaluate(objs)[objectives[0]].Penalty, s.Evaluate(objs)[objectives[1]].Penalty, dominated, s.N));

            }

            


        }
    }

    class Objective1 : IObjective
    {
        public double Evaluate(ISolution soln)
        {
            return Math.Abs((soln as Solution1).N - 5);
        }

        public int Penalty(ISolution soln)
        {
            return Penalty(Evaluate(soln));
        }

        public int Penalty(double val)
        {
            return (int)val;
        }
    }
    class Objective2 : IObjective
    {
        public double Evaluate(ISolution soln)
        {
            return Math.Abs((soln as Solution1).N - 8);
        }

        public int Penalty(ISolution soln)
        {
            return Penalty(Evaluate(soln));
        }

        public int Penalty(double val)
        {
            return (int)val;
        }
    }
    class Solution1 : BaseSolution
    {
        static Random rand = new Random();
        public int N = rand.Next(10);

        public override bool IsDuplicate(ISolution soln)
        {
            bool ret = (this.N == ((Solution1)soln).N);
            return ret;
        }
    }
}
