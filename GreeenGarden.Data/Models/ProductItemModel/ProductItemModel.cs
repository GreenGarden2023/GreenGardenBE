namespace GreeenGarden.Data.Models.ProductItemModel
{
    public class ProductItemModel
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public double? price { get; set; }
        public string status { get; set; }
        public string description { get; set; }
        public Guid? subProductId { get; set; }
        public List<string> imgUrl { get; set; }
    }

    public class ProductItemCreateRequestModel
    {
        public double price { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public Guid subProductId { get; set; }

    }
}
