namespace GreeenGarden.Data.Models.OrderModel
{
    public class OrderCreateModel
    {

        public DateTime StartDateRent { get; set; }

        public DateTime EndDateRent { get; set; }

        public int ShippingID { get; set; }

        public int? RewardPointUsed { get; set; }

        public string? RecipientAddress { get; set; }

        public string? RecipientPhone { get; set; }

        public string? RecipientName { get; set; }

        public Guid? RentOrderGroupID { get; set; }

        public List<OrderDetailModel> ItemList { get; set; }

    }
    public class OrderDetailModel
    {
        public Guid ProductItemDetailID { get; set; }
        public int Quantity { get; set; }

    }
}

