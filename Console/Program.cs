﻿using System;
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

            var solutions = new List<ISolution>();
            for (int i = 0; i < 100; i++)
            {
                var s = new Solution1();
                if (!s.IsDuplicate(solutions))
                {
                    solutions.Add(s);
                }
            }

            var objs = new List<IObjective>();
            for (int i = 0; i < 15; i++) { objs.Add(new TargetObjective(i) { ValueProvider = (parm) => { return ((Solution1)parm).N; } }); }

 
            var store = solver.DataStore;
            store.AddCollection<List<IObjective>, IObjective>(objs);
            store.AddCollection<List<ISolution>, ISolution>(solutions);

            store.Add<Filter>(Filter1);
            store.Add<Filter>(Filter2);

            //Console.WriteLine(c.GetReadOnlyCollection<IObjective>());
            //Console.WriteLine(c.GetReadOnlyCollection<ISolution>());

            //var o1 = c.GetReadOnlyCollection<IObjective>();
            ////o1.Add(new Objective1());
            //foreach (var o2 in c.GetEnumerable<IObjective>())
            //{
            //    Console.WriteLine(o2);
            //}

            //var objs = c.GetEnumerable<IObjective>();

            solver.ApplyFilter(Filter1);
            solver.ApplyFilter(Filter2);
            solver.RemoveFilteredSolutions();

            var solns = store.GetReadOnlyCollection<ISolution>();
            foreach (Solution1 s in solns)
            {
                var dominated = s.IsDominated(solns, objs);
                var evals = s.Evaluate(objs);
                foreach (var eval in evals.Values)
                {
                    Console.Write(eval.Penalty + " ");
                }
                
                Console.WriteLine(String.Format(dominated +" "+ s.N));
                store.Add<ISolution>(new Solution1());
            }
        }

        public static bool Filter1(CollectionManager cm, ISolution soln)
        {
            // flag any solutions that have an undesired objective penalty
            var objs = cm.GetEnumerable<IObjective>();
            return objs.Any(obj => soln.Evaluate(obj).Penalty > 11);
        }
        public static bool Filter2(CollectionManager cm, ISolution soln)
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


    class Objective1 : IObjective
    {
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
        static Random rand = new Random();
        public int N = rand.Next(10);

        public override bool IsDuplicate(ISolution soln)
        {
            bool ret = (this.N == ((Solution1)soln).N);
            return ret;
        }
    }
}