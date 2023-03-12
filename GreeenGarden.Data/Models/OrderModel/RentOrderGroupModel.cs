using System;
namespace GreeenGarden.Data.Models.OrderModel
{
	public class RentOrderGroupModel
	{
        public DateTime CreateDate { get; set; }
        public double TotalGroupAmount { get; set; }
        public int NumberOfOrder { get; set; }
		public List<RentOrderResModel> RentOrderList { get; set; }
		
    }
}

