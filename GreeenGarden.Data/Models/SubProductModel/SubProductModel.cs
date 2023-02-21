namespace GreeenGarden.Data.Models.SubProductModel
{
    public class SubProductModel
    {
    }
    public class SizeItemRequestModel
    {
        public Guid productId { get; set; }
        public Guid sizeId { get; set; }
        public string name { get; set; }
        public float minPrice { get; set; }
        public float maxPrice { get; set; }
        public float price { get; set; }

    }

    public class SubProductAndSize
    {
        public Guid subProductId { get; set; }
        public string name { get; set; }
        public double? price { get; set; }
        public string size { get; set; }
        public int quantity { get; set; }
        public double? minPrice { get; set; }
        public double? maxPrice { get; set; }
    }
}
