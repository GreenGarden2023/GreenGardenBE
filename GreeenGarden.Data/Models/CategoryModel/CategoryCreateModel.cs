using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GreeenGarden.Data.Models.CategoryModel
{
	public class CategoryCreateModel
	{
        [Required]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public IFormFile? Image { get; set; }

    }
}

