using System;
using Microsoft.AspNetCore.Http;

namespace GreeenGarden.Data.Models.CategoryModel
{
	public class CategoryCreateModel
	{
        public string? Name { get; set; }

        public string? Description { get; set; }

        public IFormFile? Image { get; set; }

    }
}

