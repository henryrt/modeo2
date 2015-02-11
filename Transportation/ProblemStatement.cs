using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTH.Modeo2
{
    public class ProblemStatement
    {
        public DateTime StartDate;

        public List<Order> Orders;
        public List<Destination> Destinations;
        public List<Rate> Rates;
        public List<VehicleType> VehicleTypes;
        public List<Customer> Customers;
    }
}
