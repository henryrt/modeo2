using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public class TransportationPlan : BaseSolution
    {
        public ProblemStatement Problem;
        public List<Trip> Trips { get; } = new List<Trip>();

        public TransportationPlan(ProblemStatement prob)
        {
            Problem = prob;
        }
        public void AddTrip(Trip trip)
        {
            if (!trip.InPlan) // if not already in a plan, compute
            {
                trip.FindRate();
                trip.Compute();
            }
            Trips.Add(trip);
            trip.InPlan = true;
        }

        //cache the ToString
        private string _tostring = null;

        public override string ToString()
        {
            // output a human readable plan

            if (_tostring == null)
            {
                var buf = new StringBuilder();

                var tripNum = 1;
                // order the trips by departure date
                foreach (var trip in Trips.OrderBy(t => t.DepartureDate).ThenBy(t => t.Destination.Name).ThenBy(t => t.Vehicle.Name).ThenByDescending(t => t.Tons))
                {
                    buf.AppendFormat("Vehicle# {0,-3}\n", tripNum++);
                    buf.AppendLine(Output(trip));
                }
                _tostring = buf.ToString();
            }
            return _tostring;
        }

        public override bool Equals(object obj)
        {
            var item = obj as TransportationPlan;
            if (item == null) return false;

            return ToString().Equals(item.ToString());
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        private string Output(Trip trip)
        {
            var buf = new StringBuilder();
            string t1 = "{0,-16}{1,-12}{2,-12}{3,-6}{4,-8}{5,-16}";
            string f1 = "{0:MM/dd/yy HH:mm}  {1,-12}{2,-12}{3,4}  {4,-8}{5:MM/dd/yy HH:mm}";
            buf.AppendLine(String.Format(t1, "Dep Date", "Destination", "Vehicle", "Tons", "Cost", "Arr Date"));
            buf.AppendLine(String.Format(f1,trip.DepartureDate, trip.Destination.Name, trip.Vehicle.Name, trip.Tons, trip.Cost, trip.ArrivalDate));
            buf.AppendLine();
            string f2 = "ID: {0,-8} Customer: {2,-10} Tons: {1,4}     Due: {3:MM/dd/yy HH:mm} {4}";
            foreach(var shipment in trip.Shipments.OrderBy(s => s.Order.ID))
            {
                var lateflag = trip.ArrivalDate > shipment.Order.DueDate ? "*" : "";
                buf.AppendLine(String.Format(f2, shipment.Order.ID, shipment.Tons, shipment.Order.Customer.Name, shipment.Order.DueDate, lateflag));
            }
            buf.AppendLine("-----------------------------------------------");
            return buf.ToString();
        }

        public string OutputByOrder()
        {
            var sb = new StringBuilder();
            //get all shipments and order by OrderID
            var records = new Dictionary<Shipment, Trip>();
            foreach(var trip in Trips.OrderBy(t => t.DepartureDate))
            {
                foreach (var shipment in trip.Shipments) records.Add(shipment, trip);
            }
            foreach(var r in records.Keys.OrderBy(s => s.Order.ID))
            {
                var trip = records[r];
                var late = trip.ArrivalDate > r.Order.DueDate;

                var lateflag = late ? (r.Order.Customer.Special ? "S" : "*") : "";
                sb.AppendFormat("{0,8} {1,-12} {2,-12} {3:MM/dd/yy HH:mm}  {4,-5} Tons {5}", r.Order.ID, trip.Destination.Name, trip.Vehicle.Name, trip.ArrivalDate, r.Tons, lateflag);
                //sb.Append($"{r.Order.ID,8} {trip.Destination.Name,-12} {trip.Vehicle.Name,-12} {trip.ArrivalDate:MM/dd/yy HH:mm}  { r.Tons,-5} Tons {lateflag}");
                sb.AppendLine();
            }
            return sb.ToString();
        }


    }
}
