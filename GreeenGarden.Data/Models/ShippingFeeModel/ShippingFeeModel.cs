using System;
namespace GreeenGarden.Data.Models.ShippingFeeModel
{
	public class ShippingFeeInsertModel
	{
		public int ID { get; set; }
		public double FeeAmount { get; set; }
	}
    public class ShippingFeeResModel
    {
        public int ID { get; set; }
        public string District { get; set; }
        public double FeeAmount { get; set; }
    }
}

