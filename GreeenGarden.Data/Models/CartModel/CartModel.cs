using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreeenGarden.Data.Models.CartModel
{
    public class CartModel
    {

    }
    public class Items
    {
        public Guid? ProductItemId { get; set; }
        public int? Quantity { get; set; }
        public double? Price{ get; set; }
    }
    public class CartShowModel
    {
        public string? Status { get; set; }
        public double? TotalPrice { get; set; }
        public int? Quantity { get; set; }
        public bool? IsForRent { get; set; }
        public List<Items> Items { get; set; }
    }
    public class AddToCartModel
    {
        public Guid? ProductItemId { get; set; }
        public int? Quantity { get; set; }
    }
}
