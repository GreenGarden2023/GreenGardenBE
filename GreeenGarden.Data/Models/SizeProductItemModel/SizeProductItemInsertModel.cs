using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GreeenGarden.Data.Models.SizeProductItemModel
{
    public class SizeProductItemInsertModel
    {

        [Required]
        public Guid SizeId { get; set; }

        public double? RentPrice { get; set; }

        public double? SalePrice { get; set; }

        public int? Quantity { get; set; }

        public string? Content { get; set; }

        [Required]
        public string Status { get; set; } = null!;

        [Required]
        public List<IFormFile> Images { get; set; } = null!;
    }
}

