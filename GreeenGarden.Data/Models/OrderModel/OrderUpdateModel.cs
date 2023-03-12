using System;
namespace GreeenGarden.Data.Models.OrderModel
{
	public class OrderUpdateModel
	{
		public Guid orderID { get; set; }
		public string status { get; set; }
	}
}

