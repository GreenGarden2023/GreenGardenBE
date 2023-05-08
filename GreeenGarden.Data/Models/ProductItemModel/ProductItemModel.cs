using System.ComponentModel.DataAnnotations;

namespace GreeenGarden.Data.Models.ProductItemModel
{
    public class ProductItemModel
    {
        [Required]
        public Guid Id { get; set; }

        public string? Name { get; set; } = null!;

        public string? Content { get; set; }

        public string? Description { get; set; }

        public Guid? ProductId { get; set; }

        public string? Type { get; set; } = null!;

        public string? ImageURL { get; set; } = null!;

        public string? Rule { get; set; }
        public string? CareGuide { get; set; }
    }
}

