using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GreeenGarden.Data.Models.CategoryModel
{
    public class CategoryUpdateModel
    {
        [Required]
        public Guid ID { get; set; }

        public string? Name { get; set; }

        public string? Status { get; set; }

        public string? Description { get; set; }

        public IFormFile? Image { get; set; }
    }

    public class CategoryUpdateStatusModel
    {
        public Guid CategoryID { get; set; }
        public string? status { get; set; }
    }
}

