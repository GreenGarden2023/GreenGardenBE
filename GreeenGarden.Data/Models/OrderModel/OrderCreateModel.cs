namespace GreeenGarden.Data.Models.OrderModel
{
	public class OrderCreateModel
	{

		public DateTime StartDateRent { get; set; }

        public DateTime EndDateRent { get; set; }

        public int? RewardPointUsed { get; set; }

        public Guid? ReferenceOrderID { get; set; }

        public List<OrderDetailModel> ItemList { get; set; }

    }
    public class OrderDetailModel
    {
        public Guid ID { get; set; }
        public int Quantity { get; set; }

    }
}

