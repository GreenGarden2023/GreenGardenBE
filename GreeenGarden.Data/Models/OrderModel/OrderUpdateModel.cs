namespace GreeenGarden.Data.Models.OrderModel
{
    public class OrderUpdateModel
    {
        public Guid orderID { get; set; }
        public string? status { get; set; }
    }
    public class OrderCancelModel
    {
        public Guid orderID { get; set; }
        public string? orderType { get; set; }
    }
}

