using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTH.Modeo2;
using static System.Console;

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

        public void Run(String[] args)
        {
            solver = new BusSolver() { Problem = ps };
            solver.DataStore.AddCollection<Driver>(ps.Drivers);

            LoadObjectives(solver);
            LoadFilters(solver);
            LoadAlgorithms(solver);

            for (int i = 0; i < 10; i++)
            {
                new ByLineCreate().Run(solver);
                new EmptyCreate().Run(solver);
                new PrefShiftAssignments().Run(solver);
            }

            for (int i = 0; i <200; i++)
            {
                //new CopyBest(1, "LateEarly").Run(solver);
                //new CopyBest(1, "EmptyShifts").Run(solver);
                if (solver.DataStore.Count<ISolution>() < 60)
                {
                    new CopyBest(5, "Points").Run(solver);
                    new ByLineCreate().Run(solver);
                }
                if (solver.DataStore.Count<ISolution>() > 20)
                {
                    new RemoveRandomDriver().Run(solver);
                    new RemoveRandomDriver().Run(solver);
                    new RemoveRandomDriver().Run(solver);
                    new ByLineCreate().Run(solver);
                    new ByLineCreate().Run(solver);
                }
                // add a new empty soln
                for (int j = 0; j < 5; j++) new EmptyCreate().Run(solver);
                // add a new prefered shift sched
                for (int j = 0; j < 5; j++) new PrefShiftAssignments().Run(solver);

                solver.Start(1000);
                solver.RemoveFilteredSolutions(true);
                solver.RemoveDominatedSolutions();
                
                //ShowGrid(solver);
                //Console.WriteLine("Iteration {0} complete", i+1);
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
                    LimitValue = -100
                }.GetFilter(cm, soln);
            });
            solver.AddFilter((cm, soln) =>
            {
                return new ObjectiveFilter()
                {
                    ObjectiveName = "LateEarly",
                    LimitValue = 1
                }.GetFilter(cm, soln);
            });

            solver.RemoveFilteredSolutions(true);
            RemoveDuplicates(solver);
            ShowGrid(solver);

            //check for bad solutions
            foreach (var soln in solver.DataStore.GetEnumerable<ISolution>().Cast<Schedule>())
            {
                if (soln.BookingViolation()) throw new ApplicationException("Invalid schedule");
            }

            int row = 0;
            //show solutions
            foreach(var sched in solver.DataStore.GetEnumerable<ISolution>().Cast<Schedule>())
            //if (sched != null)
            {
                Console.WriteLine("Solution {0}", row++);
                foreach (var d in sched.GetDrivers())
                {
                    Console.WriteLine(sched.DisplayDriver(d));
                }
                //foreach (var a in sched.GetAssignments().OrderBy(a => a.Shift).OrderBy(a => a.Day))
                //{
                //    Console.WriteLine(a);
                //}
                var sh = sched.GetShifts();
                for(var line = 0; line < sh.GetLength(1); line++)
                {
                    for(var index = 0; index < sh.GetLength(0); index++)
                    {
                        if (sh[index, line] == null) Console.Write(" ");
                        Console.Write(sh[index, line]?.Name + " ");
                    }
                    Console.WriteLine();
                }
                //sched.GenerateAssignments();
                //foreach (var a in sched.GetAssignments().OrderBy(a => a.Shift).OrderBy(a => a.Day))
                //{
                //    Console.WriteLine(a);
                //}


            }
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
                LimitValue = 149
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
            DisplayResults(solver.getGrid(results));
        }

        public static void ShowGrid(BaseSolver solver)
        {
            var grid = solver.getGrid();

            DisplayResults(grid);
        }

        private static void DisplayResults(System.Collections.ArrayList grid)
        {

            var i = 0;
            foreach (string[] row in grid)
            {
                for (var ix = 0; ix < row.Length; ix++) Write("{0,-12}", row[ix]);
                WriteLine(" " + i++);
            }
            WriteLine(grid.Count - 1 + " solutions.");
        }

    }
}
