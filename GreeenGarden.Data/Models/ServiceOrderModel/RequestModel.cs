using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.ServiceOrderModel
{
    public class RequestModel
    {

    }
    public class RequestDetailModel
    {
        public Guid ID { get; set; }
        public string TreeName { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
    }
}
