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

            // create 3 empty schedules
            new EmptyCreate().Run(solver);
            new EmptyCreate().Run(solver);
            new EmptyCreate().Run(solver);

            //create 10 using preferred shifts
            for (int i = 0; i < 10; i++)
            {
                new PrefShiftAssignments().Run(solver);
            }

            for (int i = 0; i < 100; i++)
            {
                // add a new empty soln
                new EmptyCreate().Run(solver);
                // add a new prefered shift sched
                new PrefShiftAssignments().Run(solver);

                solver.Start(500000);
                solver.RemoveFilteredSolutions(true);
                solver.RemoveDominatedSolutions();
                ShowGrid(solver);
            }

            solver.AddFilter((cm, soln) =>
            {
                return new ObjectiveFilter()
                {
                    ObjectiveName = "EmptyShifts",
                    LimitValue = 1
                }.GetFilter(cm, soln);
            });

            solver.RemoveFilteredSolutions(true);

            //check for bad solutions
            foreach (var soln in solver.DataStore.GetEnumerable<ISolution>().Cast<Schedule>())
            {
                if (soln.BookingViolation()) throw new ApplicationException("Invalid schedule");
            }

            foreach(var sched in solver.DataStore.GetEnumerable<ISolution>().Cast<Schedule>())
            //if (sched != null)
            {
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

        private void LoadFilters(BusSolver solver)
        {
            var filter1 = new ObjectiveFilter()
            {
                ObjectiveName = "EmptyShifts",
                LimitValue = 4
            };
            solver.AddFilter((cm, soln) =>
            {
                return filter1.GetFilter(cm, soln);
            });

            var filter2 = new ObjectiveFilter()
            {
                ObjectiveName = "Points",
                LimitValue = 333
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
            solver.DataStore.Add<IObjective>(new TargetObjective("Points", 0, 0, 1)
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
