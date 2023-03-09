namespace GreeenGarden.Data.Models.ProductItemModel
{
    public class ProductItemModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public Guid ProductId { get; set; }

        public string Type { get; set; } = null!;

        public string Image { get; set; } = null!;

    }
}

