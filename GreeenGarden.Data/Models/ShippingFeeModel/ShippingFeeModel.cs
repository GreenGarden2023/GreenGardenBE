namespace GreeenGarden.Data.Models.ShippingFeeModel
{
    public class ShippingFeeInsertModel
    {
        public int DistrictID { get; set; }
        public double FeeAmount { get; set; }
    }
    public class ShippingFeeResModel
    {
        public int DistrictID { get; set; }
        public string District { get; set; }
        public double FeeAmount { get; set; }
    }
}

