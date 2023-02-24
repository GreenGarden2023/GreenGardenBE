using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.OrderModel
{
    public class OrderModel
    {
        public List<Item> Items { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
    }

    public class Item
    {
        public Guid productId { get; set; }
        public int quantity { get; set; }
    }
}
