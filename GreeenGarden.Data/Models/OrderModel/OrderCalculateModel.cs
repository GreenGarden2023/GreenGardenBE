using System;
namespace GreeenGarden.Data.Models.OrderModel
{
	public class OrderCalculateModel
	{
		public double? TransportFee { get; set; }
        public double? Deposit { get; set; }
        public double? TotalPrice { get; set; }
        public double? RemainMoney { get; set; }
        public int? RewardPointGain { get; set; }
        public int? RewardPointUsed { get; set; }
        public double? DiscountAmount { get; set; }
    }
}

