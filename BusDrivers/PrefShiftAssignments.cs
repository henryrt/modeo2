using System;
using RTH.Modeo2;

namespace RTH.BusDrivers
{
    internal class PrefShiftAssignments : IAlgorithm
    {
        public void Run(ISolver solver)
        {
            // create a new solution and assign the prefered shifts for 5 random drivers
            var s = solver as BusSolver;
            var schedule = s.NewSchedule(algName: "PrefShiftAssignments");
            s.DataStore.Add<ISolution>(schedule);
            
            for (int i = 0; i<5; i++)
            {
                var d = s.DataStore.GetRandom<Driver>();
                var shifts = d.PrefShift;
                foreach (var shift in shifts)
                {

                    var a = new Assignment()
                    {
                        Driver = d,
                        Day = shift / 2,
                        Shift = shift % 2,
                        Line = s.DataStore.GetRandom(d.Lines)
                    };

                    schedule.SetShift(a);
                }
            }
            //if (schedule.BookingViolation()) throw new ApplicationException("Invalid Schedule created.");
        }
    }
}