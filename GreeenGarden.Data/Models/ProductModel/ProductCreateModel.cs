using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GreeenGarden.Data.Models.ProductModel
{

    public class ProductCreateModel
    {
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
        [Required]
        public IFormFile? ImgFile { get; set; }
        public bool? IsForSale { get; set; }
        public bool? IsForRent { get; set; }

    }
}
