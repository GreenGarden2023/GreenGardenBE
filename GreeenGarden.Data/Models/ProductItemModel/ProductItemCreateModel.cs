using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GreeenGarden.Data.Models.ProductItemModel
{
	public class ProductItemCreateModel
	{
        [Required]
        public string? Name { get; set; }

        public double? SalePrice { get; set; }

        [Required]
        public string? Status { get; set; }

        public string? Description { get; set; }

        [Required]
        public Guid? ProductId { get; set; }

        [Required]
        public Guid? SizeId { get; set; }

        public int? Quantity { get; set; }

        [Required]
        public string? Type { get; set; }

        public double? RentPrice { get; set; }

        public string? Content { get; set; }

        [Required]
        public List<IFormFile>? Images { get; set; }
    }
}

