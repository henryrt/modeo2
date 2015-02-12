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

    // Trip records can be used by more than one Plan
    // Do not modify them once they are in a plan
    public class Trip
    {
        public Destination Destination;
        public VehicleType Vehicle;
        public DateTime DepartureDate;
        public List<Shipment> Shipments;

        public double Cost;
        public DateTime ArrivalDate;
        public int Tons;
        public bool InPlan = false;
        public Rate Rate;


        // call FindRate once Destination, DepartureDate, and VehicleType are set.
        public void FindRate()
        {
            Rate = Destination.Rates.Find(r => r.VehicleType == Vehicle);
            // compute arrival
            ArrivalDate = DepartureDate.Add(Rate.Duration);
        }

        // call Compute once Shipments are set.
        public void Compute()
        {
            Tons = Shipments.Sum(s => s.Tons);
            Cost = Rate.CostFunction(Tons);
        } 

        // creates an new identical Trip with a different departure date
        public Trip MoveDepartureDate(DateTime newDepartureDate)
        {
            var trip = new Trip()
            {
                DepartureDate = newDepartureDate,
                Destination = this.Destination,
                Vehicle = this.Vehicle,
                Shipments = this.Shipments                
            };
            return trip;
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
