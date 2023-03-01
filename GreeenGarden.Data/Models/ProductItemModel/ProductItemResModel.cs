using System;
using GreeenGarden.Data.Models.SizeProductItemModel;
using System.ComponentModel.DataAnnotations;

namespace GreeenGarden.Data.Models.ProductItemModel
{
	public class ProductItemResModel
	{
        public Guid Id { get; set; } 
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public Guid ProductId { get; set; }
        public string Type { get; set; } = null!;
        public List<SizeProductItemResModel> sizeModelList { get; set; } = null!;
    }
}

