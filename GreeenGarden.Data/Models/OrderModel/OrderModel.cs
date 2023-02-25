using GreeenGarden.Data.Models.AddendumModel;
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
        public string Address { get; set; }
    }

    public class Item
    {
        public Guid productId { get; set; }
        public int quantity { get; set; }
    }

    public class addendumProductItemRequestModel
    {
        public int Quantity { set; get; }
        public Guid ProductItemID { get; set; }
    }

    public class addendumToAddByOrderModel
    {
        public Guid OrderId { set; get; }
        public DateTime? StartDateRent { set; get; }
        public DateTime? EndDateRent { set; get; }
        public string? Address { set; get; }
        public List<addendumProductItemRequestModel>? ProductItems { get; set; }

    }
    public class listOrderResponseModel
    {
        public Guid OrderId { set; get; }
        public double? TotalPrice { set; get; }
        public DateTime? CreateDate { set; get; }
        public string? Status { set; get; }
        public Guid? VoucherID { set; get; }
    }
}
