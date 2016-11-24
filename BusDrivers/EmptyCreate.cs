using RTH.Modeo2;

namespace RTH.BusDrivers
{
    internal class EmptyCreate : IAlgorithm
    {
        public void Run(ISolver solver)
        {            
            var s = solver as BusSolver;
            var schedule = s.NewSchedule(algName: "EmptyCreate");
            s.DataStore.Add<ISolution>(schedule);
        }
    }
}