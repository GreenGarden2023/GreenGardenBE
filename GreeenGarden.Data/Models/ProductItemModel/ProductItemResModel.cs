using GreeenGarden.Data.Models.ProductItemDetailModel;

namespace GreeenGarden.Data.Models.ProductItemModel
{
    public class ProductItemResModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Content { get; set; }
        public Guid ProductId { get; set; }
        public string Type { get; set; } = null!;
        public string ImageURL { get; set; } = null!;
        public string Rule { get; set; } 
        public List<ProductItemDetailResModel> ProductItemDetail { get; set; } = null!;
    }
}

