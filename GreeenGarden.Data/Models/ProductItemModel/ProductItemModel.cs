namespace GreeenGarden.Data.Models.ProductItemModel
{
    public class ProductItemModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public double? SalePrice { get; set; }

        public string? Status { get; set; }

        public string? Description { get; set; }

        public Guid? ProductId { get; set; }

        public Guid? SizeId { get; set; }

        public int? Quantity { get; set; }

        public string? Type { get; set; }

        public double? RentPrice { get; set; }

        public string? Content { get; set; }

        public List<string>? ImgURLs { get; set; }
    }
}
