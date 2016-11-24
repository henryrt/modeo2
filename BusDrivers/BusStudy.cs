using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTH.Modeo2;
using static System.Console;
using System.IO;

namespace RTH.BusDrivers
{
    public class BusStudy
    {
        private ProblemStatement ps;

        private BusSolver solver;
        public BusStudy(ProblemStatement ps)
        {
            this.ps = ps;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public void Run(String[] args)
        {
            solver = new BusSolver() { Problem = ps };
            solver.DataStore.AddCollection<Driver>(ps.Drivers);

            LoadObjectives(solver);
            LoadFilters(solver);
            LoadAlgorithms(solver);
            LoadSeedSolutions(solver);
            ShowGrid(solver);
            solver.RemoveDominatedSolutions();
            ShowGrid(solver);

            for (int i = 0; i < 10; i++)
            {
                new ByLineCreate().Run(solver);
                new EmptyCreate().Run(solver);
                new PrefShiftAssignments().Run(solver);
            }

            ShowGrid(solver);
            for (int i = 0; i < 1000; i++)
            {
                //new CopyBest(1, "LateEarly").Run(solver);
                //new CopyBest(1, "EmptyShifts").Run(solver);
                if (solver.DataStore.Count<ISolution>() < 70)
                {
                    new CopyBest(10, "Points").Run(solver);
                    //new CopyBest(5, "LongRest").Run(solver);
                    new ByLineCreate().Run(solver);
                    new ByLineCreate().Run(solver);
                    new ByLineCreate().Run(solver);
                    new ByLineCreate().Run(solver);
                    new ByLineCreate().Run(solver);
                    new PrefShiftAssignments().Run(solver);
                    new PrefShiftAssignments().Run(solver);
                    new PrefShiftAssignments().Run(solver);
                    new PrefShiftAssignments().Run(solver);
                    new PrefShiftAssignments().Run(solver);
                    new PrefShiftAssignments().Run(solver);
                }
                if (solver.DataStore.Count<ISolution>() < 80)
                {
                    new CrossCreate().Run(solver);
                    new CrossCreate().Run(solver);
                    new CrossCreate().Run(solver);
                    new CrossCreate().Run(solver);
                    new CrossCreate().Run(solver);
                }

                if (solver.DataStore.Count<ISolution>() > 50)
                {
                    new RemoveRandomDriver().Run(solver);
                    new RemoveRandomDriver().Run(solver);
                    new RemoveRandomDriver().Run(solver);
                    new RemoveRandomDriver().Run(solver);
                    new RemoveRandomDriver().Run(solver);
                    //new ByLineCreate().Run(solver);
                    //new ByLineCreate().Run(solver);
                }
                // add a new empty soln
                //for (int j = 0; j < 5; j++) new EmptyCreate().Run(solver);
                // add a new prefered shift sched
                //for (int j = 0; j < 10; j++) new PrefShiftAssignments().Run(solver);

                solver.Start(2000);
                solver.RemoveFilteredSolutions(true);
                solver.RemoveDominatedSolutions();

                if (i % 100 == 0) ShowGrid(solver);

                var best = solver.BestSchedule("Points");
                var points = best?.Evaluate("Points");
                Console.WriteLine("Iteration {0} complete\t\tPopulation = {1}  \t[best schedule {2},  \t{3} points  \t{4}",
                    i + 1, solver.getGrid().Count - 1, best?.ToString(), points?.Penalty, "");// best?.Algorithm);
            }
            solver.AddFilter((cm, soln) =>
            {
                return new ObjectiveFilter()
                {
                    ObjectiveName = "EmptyShifts",
                    LimitValue = 0
                }.GetFilter(cm, soln);
            });
            solver.AddFilter((cm, soln) =>
            {
                return new ObjectiveFilter()
                {
                    ObjectiveName = "Points",
                    LimitValue = -111
                }.GetFilter(cm, soln);
            });
            solver.AddFilter((cm, soln) =>
            {
                return new ObjectiveFilter()
                {
                    ObjectiveName = "LateEarly",
                    LimitValue = 0
                }.GetFilter(cm, soln);
            });

            ShowGrid(solver);
            solver.RemoveDominatedSolutions();
            ShowGrid(solver);
            solver.RemoveFilteredSolutions(true);
            RemoveDuplicates(solver);
            ShowGrid(solver);
            solver.RemoveDominatedSolutions();
            ShowGrid(solver);

            WriteSolutions(solver);
            //check for bad solutions
            //foreach (var soln in solver.DataStore.GetEnumerable<ISolution>().Cast<Schedule>())
            //{
            //    if (soln.BookingViolation()) throw new ApplicationException("Invalid schedule");
            //}

            ShowSolutions();
            //if (solver.DataStore.Count<ISolution>() > 0)
            //{
            //    solver.DataStore.RemoveAll<IAlgorithm>();
            //    solver.DataStore.Add<IAlgorithm>(new CrossCreate());
            //    //solver.DataStore.Add<IAlgorithm>(new RemoveRandomDriver());

            //    for (int i = 0; i < 100; i++)
            //    {
            //        solver.Start(30);
            //        //ShowGrid(solver);
            //        solver.RemoveFilteredSolutions(true);
            //        RemoveDuplicates(solver);
            //        solver.RemoveDominatedSolutions();
            //    }
            //    ShowGrid(solver);

            //}
            //ShowSolutions();
        }

        private void LoadSeedSolutions(BusSolver solver)
        {
            try
            {
                // read seed solutions
                var sr = new StreamReader("solutions.txt");
                while (!sr.EndOfStream)
                {
                    var input = new string[3];
                    for (int i = 0; i < 3; i++) input[i] = sr.ReadLine();
                    var solution1 = solver.NewSchedule(algName: "Seed");
                    solution1.PopulateSchedule(input);
                    solver.AddSolution(solution1);
                }
                sr.Close();
                //for (int i = 0; i < 3; i++)
                //    Console.WriteLine(input[i]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

            }
        }

        private void ShowSolutions()
        {
            int row = 1;
            //show solutions
            foreach (var sched in solver.DataStore.GetEnumerable<ISolution>().Cast<Schedule>())
            //if (sched != null)
            {
                Console.WriteLine("Solution {0}", row++);
                foreach (var d in sched.GetDrivers())
                {
                    Console.WriteLine(sched.DisplayDriver(d));
                }
                var sh = sched.GetShifts();
                for (var line = 0; line < sh.GetLength(1); line++)
                {
                    for (var index = 0; index < sh.GetLength(0); index++)
                    {
                        if (sh[index, line] == null) Console.Write(" ");
                        Console.Write(sh[index, line]?.Name + " ");
                    }
                    Console.WriteLine();
                }
            }
        }

        private void WriteSolutions(BusSolver solver)
        {
            var writer = new StreamWriter("solutions.txt", false); // overwrite

            foreach (var sched in solver.DataStore.GetEnumerable<ISolution>().Cast<Schedule>())
            {
                var sh = sched.GetShifts();
                for (var line = 0; line < sh.GetLength(1); line++)
                {
                    for (var index = 0; index < sh.GetLength(0); index++)
                    {
                        if (sh[index, line] == null) Console.Write(" ");
                        writer.Write(sh[index, line]?.Name + " ");
                    }
                    writer.WriteLine();
                }
            }
            writer.Close();
        }
        private void RemoveDuplicates(BusSolver solver)
        {
            var uniques = new List<ISolution>();
            foreach (var soln in solver.DataStore.GetEnumerable<ISolution>())
            {
                if (!(soln as Schedule).IsDuplicate(uniques))
                {
                    uniques.Add(soln);
                }
            }
            //remove the solutions and only add uniques
            solver.DataStore.RemoveAll<ISolution>();
            solver.DataStore.AddCollection<ISolution>(uniques);
        }

        private void LoadFilters(BusSolver solver)
        {
            var filter1 = new ObjectiveFilter()
            {
                ObjectiveName = "EmptyShifts",
                LimitValue = 1
            };
            solver.AddFilter((cm, soln) =>
            {
                return filter1.GetFilter(cm, soln);
            });

            var filter2 = new ObjectiveFilter()
            {
                ObjectiveName = "Points",
                LimitValue = 50
            };
            solver.AddFilter((cm, soln) =>
            {
                return filter2.GetFilter(cm, soln);
            });
        }

        private void LoadAlgorithms(BusSolver solver)
        {
            //solver.DataStore.Add<IAlgorithm>(new EmptyCreate());
            //solver.DataStore.Add<IAlgorithm>(new PrefShiftAssignments());

            //            solver.DataStore.Add<IAlgorithm>(new LongWeekend());

            //solver.DataStore.Add<IAlgorithm>(new RemoveRandomDriver());
            solver.DataStore.Add<IAlgorithm>(new RandomAssignments());
            solver.DataStore.Add<IAlgorithm>(new RandomAssignments(true));
            solver.DataStore.Add<IAlgorithm>(new ReduceLateEarly());
            solver.DataStore.Add<IAlgorithm>(new ReduceExcessLates());
            solver.DataStore.Add<IAlgorithm>(new RandomAssignments());
        }

        private void LoadObjectives(BusSolver solver)
        {
            solver.DataStore.Add<IObjective>(new TargetObjective("EmptyShifts", 0, 0, 20)
            {
                ValueProvider = Objectives.UnassignedShifts
            });
            solver.DataStore.Add<IObjective>(new TargetObjective("ShiftPrefs", 0, 0, -3)
            {
                ValueProvider = Objectives.ShiftPrefs
            });
            solver.DataStore.Add<IObjective>(new TargetObjective("DayOffPref", 0, 0, -4)
            {
                ValueProvider = Objectives.DayOffPrefs
            });
            solver.DataStore.Add<IObjective>(new TargetObjective("LongRest", 0, 0, -5)
            {
                ValueProvider = Objectives.LongRest
            });
            solver.DataStore.Add<IObjective>(new TargetObjective("ExcessLate", 0, 0, 8)
            {
                ValueProvider = Objectives.ExcessLateShifts
            });
            solver.DataStore.Add<IObjective>(new TargetObjective("ConsecLate", 0, 0, 10)
            {
                ValueProvider = Objectives.ConsecutiveLateShifts
            });
            solver.DataStore.Add<IObjective>(new TargetObjective("LateEarly", 0, 0, 30)
            {
                ValueProvider = Objectives.LateFollowedByEarly
            });
            solver.DataStore.Add<IObjective>(new TargetObjective("Points", 0, -1, 1)
            {
                ValueProvider = (soln) => {
                    var objs = solver.DataStore.GetEnumerable<IObjective>().Where(o => !o.Name.Equals("Points"));
                    return (int) objs.Sum(o => o.Penalty(soln));
                    }
            });



        }

        private void ShowGrid(BaseSolver solver, IOrderedEnumerable<ISolution> results)
        {
            DisplayResults(solver.getGrid(results), solver);
        }

        public static void ShowGrid(BaseSolver solver)
        {
            var grid = solver.getGrid();

            DisplayResults(grid, solver);
        }

        private static void DisplayResults(System.Collections.ArrayList grid, BaseSolver solver)
        {

            var i = 0;
            var solns = solver.DataStore.GetEnumerable<ISolution>();
            foreach (string[] row in grid)
            {
                for (var ix = 0; ix < row.Length; ix++) Write("{0,-12}", row[ix]);
                if (i>0) Write(solns.ElementAt(i-1).ToString());
                WriteLine();
                i++;
            }
            WriteLine(grid.Count - 1 + " solutions.");
        }

    }
}
