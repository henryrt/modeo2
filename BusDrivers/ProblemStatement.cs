using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTH.Modeo2;

namespace RTH.BusDrivers
{
    public class ProblemStatement
    {
        public int NumDays = 14;
        public int NumLines = 3;
        public List<Driver> Drivers = new List<Driver>();


        public static ProblemStatement CreateProblem()
        {
            var ps = new ProblemStatement();

            ps.Drivers.Add(
                new Driver("A")
                .AddLine(2)
                .AddDayOff(3, 4, 10, 11)
                .AddPrefDayOff(8, 14)
                .AddPrefShift(2, 7)
                .AddPrefShift(1, 9, 12, 13)
               );

            ps.Drivers.Add(
                new Driver("B")
                .AddLine(1)
                .AddDayOff(4, 5, 11, 12)
                .AddPrefDayOff(7, 8)
                .AddPrefShift(2, 1, 9)
                .AddPrefShift(1, 6)
                );

            ps.Drivers.Add(
                new Driver("C")
                .AddLine(1)
                .AddDayOff(5, 6, 12, 13)
                .AddPrefDayOff(1, 10)
                .AddPrefShift(2, 14)
                .AddPrefShift(1, 8)
                );

            ps.Drivers.Add(
                new Driver("D")
                .AddLine(2, 3)
                .AddDayOff(6, 7, 13, 14)
                .AddPrefDayOff(3, 4, 11)
                .AddPrefShift(2, 2)
                .AddPrefShift(1, 10)
                );

            ps.Drivers.Add(
                new Driver("E")
                .AddLine(1, 2)
                .AddDayOff(7, 8, 14, 1)
                .AddPrefDayOff(10, 11)
                //.AddPrefShift(2, 2)
                .AddPrefShift(1, 12, 13)
                );

            ps.Drivers.Add(
                new Driver("F")
                .AddLine(2, 3)
                .AddDayOff(8, 9, 1, 2)
                .AddPrefDayOff(6, 12)
                .AddPrefShift(2, 3, 14)
                .AddPrefShift(1, 5)
                );

            ps.Drivers.Add(
                new Driver("G")
                .AddLine(2)
                .AddDayOff(9, 10, 2, 3)
                .AddPrefDayOff(5, 6, 14)
                //.AddPrefShift(2, 3, 14)
                .AddPrefShift(1, 1, 7)
                );

            ps.Drivers.Add(
                new Driver("H")
                .AddLine(1, 3)
                .AddDayOff(10, 11, 3, 4)
                .AddPrefDayOff(1, 6, 7)
                //.AddPrefShift(2, 3, 14)
                .AddPrefShift(1, 2, 9)
                );

            ps.Drivers.Add(
                new Driver("I")
                .AddLine(2)
                .AddDayOff(11, 12, 4, 5)
                .AddPrefDayOff(14)
                .AddPrefShift(2, 6)
                //.AddPrefShift(1, 2, 9)
                );

            ps.Drivers.Add(
                new Driver("J")
                .AddLine(3)
                .AddDayOff(12, 13, 5, 6)
                .AddPrefDayOff(1, 8, 9)
                .AddPrefShift(2, 2, 3)
                .AddPrefShift(1, 11)
                );

            ps.Drivers.Add(
                new Driver("K")
                .AddLine(1)
                .AddDayOff(13, 14, 6, 7)
                .AddPrefDayOff(3, 9)
                .AddPrefShift(2, 5)
                .AddPrefShift(1, 8)
                );



            return ps;
        }

        public IEnumerable<Driver> DriversForLine(int line)
        {
            var q = Drivers.Where(d => d.Lines.Contains(line));
            return q;
        }
    }
}
