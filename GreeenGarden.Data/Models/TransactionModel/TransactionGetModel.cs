using System;
namespace GreeenGarden.Data.Models.TransactionModel
{
	public class TransactionGetByOrderModel
	{
		public Guid orderId { get; set; }

		public string orderType { get; set; }
	}
    public class TransactionGetByDateModel
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}

