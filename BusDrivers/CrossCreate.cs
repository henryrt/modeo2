using System;
using RTH.Modeo2;

namespace RTH.BusDrivers
{
    internal class CrossCreate : IAlgorithm
    {
        private static Random rand = new Random();

        public CrossCreate()
        {
        }

        public void Run(ISolver solver)
        {
            //choose two random solutions and a cross point (day)
            var s = solver as BusSolver;
            var schedule1 = s.DataStore.GetRandom<ISolution>() as Schedule;
            if (schedule1 == null) return;
            var schedule2 = s.DataStore.GetRandom<ISolution>() as Schedule;
            if (schedule2 == null) return;

            var day1 = rand.Next(s.Problem.NumDays-1);

            var newSchedule = s.NewSchedule("CrossCreate");
            s.DataStore.Add<ISolution>(newSchedule);

            for (int day = 0; day <= day1; day++)
            {
                var shift = schedule1.GetShifts();
                for (int line=0; line < s.Problem.NumLines; line++)
                {
                    newSchedule.SetShift(day, 0, line, shift[day * 2, line]);
                    newSchedule.SetShift(day, 1, line, shift[day * 2+1, line]);
                }
            }
            for (int day = day1+1; day < s.Problem.NumDays; day++)
            {
                var shift = schedule2.GetShifts();
                for (int line = 0; line < s.Problem.NumLines; line++)
                {
                    newSchedule.SetShift(day, 0, line, shift[day * 2, line]);
                    newSchedule.SetShift(day, 1, line, shift[day * 2 + 1, line]);
                }
            }
        }
    }
}