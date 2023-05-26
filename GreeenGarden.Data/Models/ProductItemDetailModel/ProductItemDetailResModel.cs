using GreeenGarden.Data.Models.SizeModel;

namespace GreeenGarden.Data.Models.ProductItemDetailModel
{
    public class ProductItemDetailResModel
    {
        public Guid Id { get; set; }
        public ProductRevenueResModel Product { get; set; }

        public SizeResModel Size { get; set; } = null!;

        public double? RentPrice { get; set; }

        public double? SalePrice { get; set; }

        public int? Quantity { get; set; }

        public double? TransportFee { get; set; }

        public string Status { get; set; } = null!;
        public string? CareGuide { get; set; } = null!;

        public List<string>? ImagesURL { get; set; }

    }

    public class ProductRevenueResModel
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string? Status { get; set; }
        public bool? IsForRent { get; set; }
        public bool? IsForSale { get; set; }
        public string? ImageURL { get; set; }
        public ProductItemRevenueResModel productItem { get; set; }

    }
    public class ProductItemRevenueResModel
    {
        public Guid ProductItemId { get; set;}
        public string ProductItemName { get; set; }
        public string? ImageURL { get; set; }
    }
}

