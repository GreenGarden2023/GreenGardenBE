using System;
namespace GreeenGarden.Data.Models.OrderModel
{
	public class RentOrderResModel
	{
        public Guid Id { get; set; }

        public double? TransportFee { get; set; }

        public DateTime StartDateRent { get; set; }

        public DateTime EndDateRent { get; set; }

        public double? Deposit { get; set; }

        public double? TotalPrice { get; set; }

        public string? Status { get; set; }

        public double? RemainMoney { get; set; }

        public int? RewardPointGain { get; set; }

        public int? RewardPointUsed { get; set; }

        public Guid? RentOrderGroupID { get; set; }

        public double? DiscountAmount { get; set; }

        public List<RentOrderDetailResModel> RentOrderDetailList { get; set; }
    }
    public class RentOrderDetailResModel
    {
        public Guid ID { get; set; }
        public Guid productItemDetailID { get; set; }
        public string productItemName { get; set; }
        public string productItemUrl { get; set; }
        public double productItemDetailTotalPrice { get; set; }
        public int Quantity { get; set; }
    }
}

