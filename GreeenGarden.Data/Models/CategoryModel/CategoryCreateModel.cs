﻿using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

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

