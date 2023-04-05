using System;
namespace GreeenGarden.Data.Models.TransactionModel
{
	public class TransactionOrderCancelModel
    {
		public Guid OrderID { get; set; }

		public string OrderType { get; set; }

		public double Amount { get; set; }

		public string Status { get; set; }

		public string TransactionType { get; set; }

        public string PaymentType { get; set; }

		public string Description { get; set; }
    }
}

