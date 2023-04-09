using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.OrderModel
{
    public class OrderRangeDateReqModel
    {
        public string fromDate { get; set; }
        public string toDate { get; set; }
    }
    public class OrderRangeDateResModel
    {
        public DateTime Date { get; set; }
        public RentOrderResModel Order { get; set; }
    }
}
