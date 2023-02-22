using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GreeenGarden.Data.Models.ProductModel
{
    public class ProductUpdateModel
    {
        [Required]
        public Guid ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public Guid CategoryId { get; set; }
        public IFormFile? Image { get; set; }
        public bool? IsForSale { get; set; }
        public bool? IsForRent { get; set; }
    }
}
