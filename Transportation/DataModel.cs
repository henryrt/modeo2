using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public class Rate
    {
        public Destination Destination;
        public VehicleType VehicleType;
        public Func<int, double> CostFunction;
        public TimeSpan Duration;
    }

    public class VehicleType
    {
        public string Name;
        public int Capacity;
        public List<Rate> Rates;

        public void SetRates(List<Rate> AllRates)
        {
            Rates = AllRates.Where<Rate>(r => r.VehicleType == this).ToList();
        }
    }

    public class Trip
    {
        public Destination Destination;
        public VehicleType Vehicle;
        public DateTime DepartureDate;
        public List<Shipment> Shipments;

        public double Cost;
        public DateTime ArrivalDate;
        public int Tons;
        private Rate Rate;

        public void FindRate()
        {
            Rate = Destination.Rates.Find(r => r.VehicleType == Vehicle);
            // compute arrival
            ArrivalDate = DepartureDate.Add(Rate.Duration);
        }
        public void Compute()
        {
            Tons = Shipments.Sum(s => s.Tons);
            Cost = Rate.CostFunction(Tons);
        } 

    }
    public class Shipment
    {
        public Order Order;
        public int Tons;
    }

    public class Order
    {
        public string ID;
        public int Tons;
        public Customer Customer;
        public Destination Destination;
        public DateTime DueDate;
    }

    public class Customer
    {
        public string Name;
        public bool Special;
        public bool SpecificDates;
    }

    public class Destination
    {
        public string Name;
        public int Distance;
        public List<Rate> Rates;

        public void SetRates(List<Rate> AllRates)
        {
            Rates = AllRates.Where<Rate>(r => r.Destination == this).ToList();
        }
    }
}
