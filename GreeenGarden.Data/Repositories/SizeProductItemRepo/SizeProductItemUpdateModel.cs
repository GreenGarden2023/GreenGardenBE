using System;
using System.ComponentModel.DataAnnotations;

namespace GreeenGarden.Data.Repositories.SizeProductItemRepo
{
	public class SizeProductItemUpdateModel
	{
        [Required]
        public Guid? Id { get; set; }

        public string? Name { get; set; } = null!;

        public Guid? SizeId { get; set; }

        public Guid? ProductItemId { get; set; }

        public double? RentPrice { get; set; }

        public double? SalePrice { get; set; }

        public int? Quantity { get; set; }

        public string? Content { get; set; }

        public string? Status { get; set; } = null!;
    }
}

