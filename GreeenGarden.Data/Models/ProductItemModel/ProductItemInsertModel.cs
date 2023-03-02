using GreeenGarden.Data.Models.SizeProductItemModel;
using System.ComponentModel.DataAnnotations;

namespace GreeenGarden.Data.Models.ProductItemModel
{
    public class ProductItemInsertModel
    {
        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public string Type { get; set; } = null!;

        [Required]
        public List<SizeProductItemInsertModel> sizeModelList { get; set; } = null!;
    }
}

