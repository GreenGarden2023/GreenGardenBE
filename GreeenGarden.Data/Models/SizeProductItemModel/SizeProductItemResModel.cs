using GreeenGarden.Data.Models.SizeModel;

namespace GreeenGarden.Data.Models.SizeProductItemModel
{
    public class SizeProductItemResModel
    {
        public Guid Id { get; set; }

        public SizeResModel Size { get; set; } = null!;

        public double? RentPrice { get; set; }

        public double? SalePrice { get; set; }

        public int? Quantity { get; set; }

        public string? Content { get; set; }

        public string Status { get; set; } = null!;

        public List<string>? ImagesURL { get; set; }

    }
}

