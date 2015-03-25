using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Console;


namespace RTH.Modeo2
{
    public class Transportation
    {
        public void Run(string[] args)
        {
            var solver = new TransportationSolver(2500);
            solver.Problem = new ProblemStatement();
            LoadProblem(solver.Problem);

            LoadStrategy(solver);

            //LoadPopulation(solver);

            //var alg = solver.DataStore.GetEnumerable<IAlgorithm>().First();
            InitializePopulation(solver);
            ShowGrid(solver);

            IObjective objViolations = GetObjective(solver, "Violation");
            IObjective objCost = GetObjective(solver, "Cost");
            IObjective objLate = GetObjective(solver, "#Late");

            for (int i = 0; i < 10; i++)
            {
                solver.Start(100);
                //ShowGrid(solver);
                solver.RemoveDominatedSolutions();
                Error.WriteLine("Iteration " + i);
            }

            WriteLine("Filtered = " + solver.DataStore.GetEnumerable<ISolution>().Where(s => s.Filtered).Count());
            if (objViolations != null)
            {
                solver.ApplyFilter((cm, soln) => soln.Evaluate(objViolations).Penalty > 3);
                WriteLine("Filtered = " + solver.DataStore.GetEnumerable<ISolution>().Where(s => s.Filtered).Count());
            }
            if (objCost != null && objLate != null)
            {
                solver.ApplyFilter((cm, soln) => (soln.Evaluate(objCost).Value > 154000) && soln.Evaluate(objLate).Value > 5);
                WriteLine("Filtered = " + solver.DataStore.GetEnumerable<ISolution>().Where(s => s.Filtered).Count());
            }
            solver.RemoveFilteredSolutions();
            WriteLine("Filtered = " + solver.DataStore.GetEnumerable<ISolution>().Where(s => s.Filtered).Count());



            //solver.RemoveDominatedSolutions();
            // sort by first objective
            var obj = solver.DataStore.GetReadOnlyCollection<IObjective>().ElementAt(0);
            var results = solver.DataStore.GetEnumerable<ISolution>().OrderBy(s => s.Evaluate(obj).Value);

            ShowGrid(solver, results);

            int counter = 1;
            foreach (var plan in solver.DataStore.GetEnumerable<ISolution>())
            {
                WriteLine("SOLUTION " + counter + " ===========================================");
                WriteLine(plan);
                WriteLine("SOLUTION " + counter + " ===========================================");
                Console.WriteLine((plan as TransportationPlan).OutputByOrder());
                WriteLine("SOLUTION " + counter + " ===========================================");
                counter++;
            }

        }

        private static IObjective GetObjective(TransportationSolver solver, string name)
        {
            return solver.DataStore.GetEnumerable<IObjective>().Where(obj2 => obj2.Name == name).FirstOrDefault();
        }

        private void ShowGrid(BaseSolver solver, IOrderedEnumerable<ISolution> results)
        {
            DisplayResults(solver.getGrid(results));
        }

        private static void ShowGrid(TransportationSolver solver)
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
                WriteLine(" "+i++);
            }
            WriteLine(grid.Count - 1 + " solutions.");
        }

        private void InitializePopulation(TransportationSolver solver)
        {
            LoadPopulation(solver);
            if (solver.DataStore.Count<ISolution>() == 0)
            {
                new GenerateByVehicleType("Express").Run(solver);
                new GenerateByVehicleType("Sm Truck").Run(solver);
                new GenerateByVehicleType("Lg Truck").Run(solver);
                new GenerateByVehicleType("Railcar").Run(solver);

                new GenerateByDueDate("").Run(solver);

            }
        }

        public virtual void LoadPopulation(TransportationSolver solver)
        {
            // none
        }

        public virtual void LoadStrategy(TransportationSolver solver)
        {
            // add objectives
            solver.DataStore.Add<IObjective>(new LateOrdersObjective());
            solver.DataStore.Add<IObjective>(new CustomerViolationsObjective());
            solver.DataStore.Add<IObjective>(new NumberOfVehiclesObjective());
            solver.DataStore.Add<IObjective>(new TripsOnStartDateObjective());
            solver.DataStore.Add<IObjective>(new CostObjective());

            solver.DataStore.Add<IObjective>(new VehicleCountByTypeObjective() { Name = "Railcar" });
            solver.DataStore.Add<IObjective>(new VehicleCountByTypeObjective() { Name = "Sm Truck" });
            solver.DataStore.Add<IObjective>(new VehicleCountByTypeObjective() { Name = "Lg Truck" });
            solver.DataStore.Add<IObjective>(new VehicleCountByTypeObjective() { Name = "Express" });

            //add algorithms
            solver.DataStore.Add<IAlgorithm>(new CombineLoadsAlgorithm());
            solver.DataStore.Add<IAlgorithm>(new MoveDeparturesAlgorithm());

            //add constraints
            solver.DataStore.Add<IConstraint>(new AllOrdersShippedConstraint());
            solver.DataStore.Add<IConstraint>(new NoEmptyShipmentConstraint());
        }

        public virtual void LoadProblem(ProblemStatement problem)
        {
            var start = new DateTime(2015, 2, 1);
            problem.StartDate = start;

            Destination 
                        NYC = new Destination() { Name = "NYC", Distance = 110 },
                        Richmond = new Destination() { Name = "Richmond", Distance = 340 };

            VehicleType
                        Small = new VehicleType() { Name = "Sm Truck", Capacity = 9 },
                        Large = new VehicleType() { Name = "Lg Truck", Capacity = 15 },
                        Express = new VehicleType() { Name = "Express", Capacity = 4 },
                        Railcar = new VehicleType() { Name = "Railcar", Capacity = 40 };
            Customer
                        ACME = new Customer() { Name = "ACME", Special = false, SpecificDates = false },
                        Marsh = new Customer() { Name = "Marsh", Special = true, SpecificDates = false },
                        Bronte = new Customer() { Name = "Bronte", Special = false, SpecificDates = true };


            problem.Destinations = new List<Destination>()
            {
                NYC, Richmond
            };

            problem.VehicleTypes = new List<VehicleType>()
            {
                Small, Large, Express, Railcar
            };

            problem.Rates = new List<Rate>()
            {
                new Rate() {  Destination = NYC, VehicleType = Small, Duration = new TimeSpan(1,0,0,0) },
                new Rate() {  Destination = NYC, VehicleType = Large, Duration = new TimeSpan(1,8,0,0) },
                new Rate() {  Destination = NYC, VehicleType = Express, Duration = new TimeSpan(0,8,0,0) },
                new Rate() {  Destination = NYC, VehicleType = Railcar, Duration = new TimeSpan(3,0,0,0) },
                new Rate() {  Destination = Richmond, VehicleType = Small, Duration = new TimeSpan(2,0,0,0) },
                new Rate() {  Destination = Richmond, VehicleType = Large, Duration = new TimeSpan(3,0,0,0) },
                new Rate() {  Destination = Richmond, VehicleType = Express, Duration = new TimeSpan(1,0,0,0) },
                new Rate() {  Destination = Richmond, VehicleType = Railcar, Duration = new TimeSpan(5,0,0,0) }
            };

            //set cost functions
            foreach (Rate rate in problem.Rates.Where(r => r.VehicleType == Small)) new CostFunction(rate) { FixedCost = 200, DistanceFactor = 0.9 };
            foreach (Rate rate in problem.Rates.Where(r => r.VehicleType == Large)) new CostFunction(rate) { FixedCost = 300, DistanceFactor = 0.87 };
            foreach (Rate rate in problem.Rates.Where(r => r.VehicleType == Express)) new CostFunction(rate) { FixedCost = 500, DistanceFactor = 1.5 };
            foreach (Rate rate in problem.Rates.Where(r => r.VehicleType == Railcar)) new CostFunction(rate) { FixedCost = 1000, DistanceFactor = 0.4 };

            foreach (var dest in problem.Destinations) dest.SetRates(problem.Rates);
            
            foreach (var vehicle in problem.VehicleTypes) vehicle.SetRates(problem.Rates);
                    
            problem.Customers = new List<Customer>()
            {
               ACME, Marsh, Bronte
            };

            problem.Orders = new List<Order>()
            {
                new Order() { ID = "1001", Customer=ACME, Destination = NYC, DueDate = start.AddDays(1), Tons = 77 },
                new Order() { ID = "1002", Customer=ACME, Destination = NYC, DueDate = start.AddDays(3), Tons = 44 },
                new Order() { ID = "1003", Customer=ACME, Destination = NYC, DueDate = start.AddDays(4), Tons = 66 },
                new Order() { ID = "1004", Customer=ACME, Destination = Richmond, DueDate = start.AddDays(3), Tons = 88 },
                new Order() { ID = "1005", Customer=ACME, Destination = Richmond, DueDate = start.AddDays(4), Tons = 123 },
                new Order() { ID = "1006", Customer=Marsh, Destination = NYC, DueDate = start.AddDays(1), Tons = 19 },
                new Order() { ID = "1007", Customer=Marsh, Destination = Richmond, DueDate = start.AddDays(2), Tons = 24 },
                new Order() { ID = "1008", Customer=Marsh, Destination = Richmond, DueDate = start.AddDays(3), Tons = 15 },
                new Order() { ID = "1009", Customer=Marsh, Destination = Richmond, DueDate = start.AddDays(4), Tons = 44 },
                new Order() { ID = "1010", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(1), Tons = 102 },
                new Order() { ID = "1011", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(2), Tons = 67 },
                new Order() { ID = "1012", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(3), Tons = 15 },
                new Order() { ID = "A00332", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(1), Tons = 3 },
                new Order() { ID = "A00333", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(1), Tons = 2 },
                new Order() { ID = "A00334", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(1), Tons = 1 },
                new Order() { ID = "A00335", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(1), Tons = 8 },
                new Order() { ID = "A00336", Customer=Bronte, Destination = Richmond, DueDate = start.AddDays(1), Tons = 11 },
                new Order() { ID = "A00337", Customer=Bronte, Destination = Richmond, DueDate = start.AddDays(1), Tons = 9 },
                new Order() { ID = "B00332", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(2), Tons = 3 },
                new Order() { ID = "B00333", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(2), Tons = 2 },
                new Order() { ID = "B00334", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(2), Tons = 1 },
                new Order() { ID = "B00335", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(2), Tons = 8 },
                new Order() { ID = "B00336", Customer=Bronte, Destination = Richmond, DueDate = start.AddDays(2), Tons = 12 },
                new Order() { ID = "B00337", Customer=Bronte, Destination = Richmond, DueDate = start.AddDays(2), Tons = 7 },
                new Order() { ID = "C00332", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(3), Tons = 3 },
                new Order() { ID = "C00333", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(3), Tons = 4 },
                new Order() { ID = "C00334", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(3), Tons = 5 },
                new Order() { ID = "C00335", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(3), Tons = 5 },
                new Order() { ID = "C00336", Customer=Bronte, Destination = Richmond, DueDate = start.AddDays(3), Tons = 6 },
                new Order() { ID = "C00337", Customer=Bronte, Destination = Richmond, DueDate = start.AddDays(3), Tons = 3 }

            };


        }
        public class CostFunction
        {
            Rate rate;
            public double FixedCost = 0;
            public double DurationFactor = 0;
            public double DistanceFactor = 0;

            public CostFunction(Rate r)
            {
                rate = r;
                rate.CostFunction = Compute;
            }

            double Compute(int tons)
            {
                return FixedCost + DurationFactor * rate.Duration.Hours * tons + DistanceFactor * rate.Destination.Distance * tons;
            }

        }
    }
}
