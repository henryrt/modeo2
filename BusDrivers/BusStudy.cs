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

            for (int i = 0; i < 1; i++)
            {
                solver.Start(100000);
                solver.RemoveFilteredSolutions(true);
                solver.RemoveDominatedSolutions();
                ShowGrid(solver);
            }

            var sched = solver.DataStore.GetEnumerable<ISolution>().Last() as Schedule;
            foreach (var d in sched.GetDrivers())
            {
                Console.WriteLine(sched.DisplayDriver(d));
            }
        }

        private void LoadFilters(BusSolver solver)
        {
            var filter1 = new ObjectiveFilter()
            {
                ObjectiveName = "EmptyShifts",
                LimitValue = 0
            };
            solver.AddFilter((cm, soln) =>
            {
                return filter1.GetFilter(cm, soln);
            });
        }

        private void LoadAlgorithms(BusSolver solver)
        {
            solver.DataStore.Add<IAlgorithm>(new RandomAssignments());
            solver.DataStore.Add<IAlgorithm>(new EmptyCreate());
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
