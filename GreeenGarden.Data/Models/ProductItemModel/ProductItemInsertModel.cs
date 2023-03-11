using System.ComponentModel.DataAnnotations;

namespace GreeenGarden.Data.Models.ProductItemModel
{
    public class ProductItemInsertModel
    {
        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? Content { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public string Type { get; set; } = null!;

        [Required]
        public string ImageURL { get; set; } = null!;


    }
}

