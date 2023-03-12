using System;
namespace GreeenGarden.Data.Models.OrderModel
{
	public class SaleOrderModel
	{
        public int? RewardPointUsed { get; set; }
        public List<SaleOrderDetailModel> ItemList { get; set; }
    }
    public class SaleOrderDetailModel
    {
        public Guid ID { get; set; }
        public int Quantity { get; set; }

    }
}

