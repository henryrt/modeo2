using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Console;
using System.Collections;

namespace RTH.Modeo2
{
    public class TransportationStudy
    {
        public TransportationSolver solver;

        public delegate void DataChangedHandler();
        public event DataChangedHandler DataChanged = delegate { };

        public void Initialize(int msec)
        {
            solver = new TransportationSolver(msec);
            solver.Problem = new ProblemStatement();
            LoadProblem(solver.Problem);
            LoadStrategy(solver);

            //default to sort by first column
            SortColumnName = solver.DataStore.GetEnumerable<IObjective>().First().Name;

            InitializePopulation(solver);

            DataChanged();
        }

        public void AddFilter(Filter f)
        {
            solver.AddFilter(f);
        }

        public int nObjectives { get { return Num<IObjective>(); } }
        public int nSolutions {  get { return Num<ISolution>(); } }

        public List<string> ObjectiveNames { get {
                return solver.DataStore.GetEnumerable<IObjective>().Select(o => o.Name).ToList();               
                } }
        public List<IObjective> Objectives
        {
            get
            {
                return solver.DataStore.GetEnumerable<IObjective>().ToList();
            }
        }

        public string SortColumnName { set; get; }
        public ArrayList SolutionGrid { get { return solver.getGrid(SortColumnName); } }
        
        private int Num<T>()
        { return solver?.DataStore.Count<T>() ?? 0; }

        public void Run(int n = int.MaxValue)
        {
            solver.Start(n);

            solver.RemoveFilteredSolutions(true);
            solver.RemoveDominatedSolutions();

            DataChanged();
        }

        public void Run(string[] args)
        {
            // new solver with timeout (msec)
            solver = new TransportationSolver(2500);

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

            for (int i = 0; i < 5; i++)
            {
                solver.Start(10);
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
            foreach (var plan in results)
            {
                //WriteLine("SOLUTION " + counter + " ===========================================");
                //WriteLine(plan);
                WriteLine("SOLUTION " + counter + " ===========================================");
                Console.WriteLine((plan as TransportationPlan).OutputByOrder());
                WriteLine("SOLUTION " + counter + " ===========================================");
                counter++;
            }

        }

        public ArrayList Analyze(IObjective objective1, IObjective objective2)
        {
            // this is the result for a grid
            var result = solver.AnalyzeTradeoff(objective1, objective2);
            // keep only these
            solver.Keep(result);
            DataChanged();
            return solver.getGrid();
            //return solver.getGrid(result);
        }

        public List<TradeoffSummary> TradeOff(IObjective objective1, IObjective objective2)
        {
            //this is the simplified results for charting
            return solver.TradeoffSummaries(objective1, objective2);
        }

        private static IObjective GetObjective(TransportationSolver solver, string name)
        {
            return solver.DataStore.GetEnumerable<IObjective>().Where(obj2 => obj2.Name == name).FirstOrDefault();
        }

        private void ShowGrid(BaseSolver solver, IOrderedEnumerable<ISolution> results)
        {
            DisplayResults(solver.getGrid(results));
        }

        public static void ShowGrid(TransportationSolver solver)
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

            // if empty population, generate some solutions
            if (solver.DataStore.Count<ISolution>() == 0)
            {
                new GenerateByVehicleType("Express").Run(solver);
                new GenerateByVehicleType("Sm Truck").Run(solver);
                new GenerateByVehicleType("Lg Truck").Run(solver);
                new GenerateByVehicleType("Railcar").Run(solver);

                new GenerateByDueDate().Run(solver);

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
            //solver.DataStore.Add<IObjective>(new CustomerViolationsObjective());
            //solver.DataStore.Add<IObjective>(new NumberOfVehiclesObjective());
            //solver.DataStore.Add<IObjective>(new TripsOnStartDateObjective());
            solver.DataStore.Add<IObjective>(new CostObjective());
            solver.DataStore.Add<IObjective>(new NumberOfTrainsObjective());

            // display only (penalty = 0)
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
                        Richmond = new Destination() { Name = "Richmond", Distance = 340 },
                        Chicago = new Destination() { Name = "Chicago", Distance = 800 },
                        NewOrleans = new Destination() { Name = "New Orleans", Distance = 1100 };

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
                NYC, Richmond, Chicago, NewOrleans
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
                new Rate() {  Destination = NYC, VehicleType = Railcar, Duration = new TimeSpan(2,0,0,0) },
                new Rate() {  Destination = Richmond, VehicleType = Small, Duration = new TimeSpan(2,0,0,0) },
                new Rate() {  Destination = Richmond, VehicleType = Large, Duration = new TimeSpan(2,0,0,0) },
                new Rate() {  Destination = Richmond, VehicleType = Express, Duration = new TimeSpan(1,0,0,0) },
                new Rate() {  Destination = Richmond, VehicleType = Railcar, Duration = new TimeSpan(3,0,0,0) },
                new Rate() {  Destination = Chicago, VehicleType = Small, Duration = new TimeSpan(3,0,0,0) },
                new Rate() {  Destination = Chicago, VehicleType = Large, Duration = new TimeSpan(3,0,0,0) },
                new Rate() {  Destination = Chicago, VehicleType = Express, Duration = new TimeSpan(1,8,0,0) },
                new Rate() {  Destination = Chicago, VehicleType = Railcar, Duration = new TimeSpan(5,0,0,0) },
                new Rate() {  Destination = NewOrleans, VehicleType = Small, Duration = new TimeSpan(4,0,0,0) },
                new Rate() {  Destination = NewOrleans, VehicleType = Large, Duration = new TimeSpan(4,0,0,0) },
                new Rate() {  Destination = NewOrleans, VehicleType = Express, Duration = new TimeSpan(1,16,0,0) },
                new Rate() {  Destination = NewOrleans, VehicleType = Railcar, Duration = new TimeSpan(6,0,0,0) }
            };

            //set cost functions
            foreach (Rate rate in problem.Rates.Where(r => r.VehicleType == Small)) new CostFunction(rate) { FixedCost = 200, DistanceFactor = 0.9 };
            foreach (Rate rate in problem.Rates.Where(r => r.VehicleType == Large)) new CostFunction(rate) { FixedCost = 300, DistanceFactor = 0.75 };
            foreach (Rate rate in problem.Rates.Where(r => r.VehicleType == Express)) new CostFunction(rate) { FixedCost = 500, DistanceFactor = 1.5 };
            foreach (Rate rate in problem.Rates.Where(r => r.VehicleType == Railcar)) new CostFunction(rate) { FixedCost = 1000, DistanceFactor = 0.4 };

            foreach (var dest in problem.Destinations) dest.SetRates(problem.Rates);

            foreach (var vehicle in problem.VehicleTypes) vehicle.SetRates(problem.Rates);

            problem.Customers = new List<Customer>()
            {
               ACME, Marsh, Bronte
            };

            //problem.Orders = new List<Order>()
            //{
            //    new Order() { ID = "1001", Customer=ACME, Destination = NYC, DueDate = start.AddDays(1), Tons = 77 },
            //    new Order() { ID = "1002", Customer=ACME, Destination = NYC, DueDate = start.AddDays(3), Tons = 44 },
            //    new Order() { ID = "1003", Customer=ACME, Destination = NYC, DueDate = start.AddDays(4), Tons = 66 },
            //    new Order() { ID = "1004", Customer=ACME, Destination = Richmond, DueDate = start.AddDays(3), Tons = 88 },
            //    new Order() { ID = "1005", Customer=ACME, Destination = Richmond, DueDate = start.AddDays(4), Tons = 123 },
            //    new Order() { ID = "1006", Customer=Marsh, Destination = NYC, DueDate = start.AddDays(1), Tons = 19 },
            //    new Order() { ID = "1007", Customer=Marsh, Destination = Richmond, DueDate = start.AddDays(2), Tons = 24 },
            //    new Order() { ID = "1008", Customer=Marsh, Destination = Richmond, DueDate = start.AddDays(3), Tons = 15 },
            //    new Order() { ID = "1009", Customer=Marsh, Destination = Richmond, DueDate = start.AddDays(4), Tons = 44 },
            //    new Order() { ID = "1010", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(1), Tons = 102 },
            //    new Order() { ID = "1011", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(2), Tons = 67 },
            //    new Order() { ID = "1012", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(3), Tons = 15 },
            //    new Order() { ID = "A00332", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(1), Tons = 3 },
            //    new Order() { ID = "A00333", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(1), Tons = 2 },
            //    new Order() { ID = "A00334", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(1), Tons = 1 },
            //    new Order() { ID = "A00335", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(1), Tons = 8 },
            //    new Order() { ID = "A00336", Customer=Bronte, Destination = Richmond, DueDate = start.AddDays(1), Tons = 11 },
            //    new Order() { ID = "A00337", Customer=Bronte, Destination = Richmond, DueDate = start.AddDays(1), Tons = 9 },
            //    new Order() { ID = "B00332", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(2), Tons = 3 },
            //    new Order() { ID = "B00333", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(2), Tons = 2 },
            //    new Order() { ID = "B00334", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(2), Tons = 1 },
            //    new Order() { ID = "B00335", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(2), Tons = 8 },
            //    new Order() { ID = "B00336", Customer=Bronte, Destination = Richmond, DueDate = start.AddDays(2), Tons = 12 },
            //    new Order() { ID = "B00337", Customer=Bronte, Destination = Richmond, DueDate = start.AddDays(2), Tons = 7 },
            //    new Order() { ID = "C00332", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(3), Tons = 3 },
            //    new Order() { ID = "C00333", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(3), Tons = 4 },
            //    new Order() { ID = "C00334", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(3), Tons = 5 },
            //    new Order() { ID = "C00335", Customer=Bronte, Destination = NYC, DueDate = start.AddDays(3), Tons = 5 },
            //    new Order() { ID = "C00336", Customer=Bronte, Destination = Richmond, DueDate = start.AddDays(3), Tons = 6 },
            //    new Order() { ID = "C00337", Customer=Bronte, Destination = Richmond, DueDate = start.AddDays(3), Tons = 3 }
            //};
            problem.Orders = Enumerable.Range(0, 100).Select(i => RandomOrder(problem, i)).ToList<Order>();

        }

        private Random rand = new Random();
        private Order RandomOrder(ProblemStatement problem, int i)
        {
            var order = new Order()
            {
                ID = i.ToString(),
                Customer = problem.Customers[rand.Next(problem.Customers.Count())],
                Destination = problem.Destinations[rand.Next(problem.Destinations.Count())],
                DueDate = problem.StartDate.AddDays(2 + rand.Next(8)),
                Tons = rand.Next(50) + 1

            };
            Console.WriteLine(order);
            return order;
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
