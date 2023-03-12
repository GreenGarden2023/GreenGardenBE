namespace GreeenGarden.Data.Models.OrderModel
{
	public class RentOrderModel
	{

		public DateTime StartDateRent { get; set; }

        public DateTime EndDateRent { get; set; }

        public int? RewardPointUsed { get; set; }

        public Guid? ReferenceOrderID { get; set; }

        public List<RentOrderDetailModel> ItemList { get; set; }

    }
    public class RentOrderDetailModel
    {
        public Guid ID { get; set; }
        public int Quantity { get; set; }

    }
}

