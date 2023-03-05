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

    public class addendumSizeProductItemRequestModel
    {
        public int quantity { set; get; }
        public Guid sizeProductItemID { get; set; }
    }

    public class addendumToAddByOrderModel
    {
        public Guid orderId { set; get; }
        public DateTime? startDateRent { set; get; }
        public DateTime? endDateRent { set; get; }
        public string? address { set; get; }
        public List<addendumSizeProductItemRequestModel> sizeProductItems { get; set; }

    }
    public class listOrderResponseModel
    {
        public Guid orderId { set; get; }
        public double? totalPrice { set; get; }
        public DateTime? createDate { set; get; }
        public string? status { set; get; }
        public Guid? voucherID { set; get; }
        public bool? isForRent { set; get; }
    }
}
