
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

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
}

