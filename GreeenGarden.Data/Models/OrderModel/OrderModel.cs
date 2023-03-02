namespace GreeenGarden.Data.Models.OrderModel
{
    public class OrderModel
    {
        public List<Item> items { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string address { get; set; }
    }

    public class Item
    {
        public Guid sizeProductItemID { get; set; }
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
        public Guid orderId { set; get; }
        public double? totalPrice { set; get; }
        public DateTime? createDate { set; get; }
        public string? status { set; get; }
        public Guid? voucherID { set; get; }
    }
}
