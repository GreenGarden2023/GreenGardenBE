using GreeenGarden.Data.Models.SizeModel;

namespace GreeenGarden.Data.Models.ProductItemDetailModel
{
    public class ProductItemDetailResModel
    {
        public Guid Id { get; set; }

        public SizeResModel Size { get; set; } = null!;

        public double? RentPrice { get; set; }

        public double? SalePrice { get; set; }

        public int? Quantity { get; set; }

        public string Status { get; set; } = null!;

        public List<string>? ImagesURL { get; set; }

    }
}

