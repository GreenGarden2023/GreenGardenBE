using System.ComponentModel.DataAnnotations;

namespace GreeenGarden.Data.Models.ProductItemDetailModel
{
    public class ProductItemDetailModel
    {

        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid SizeId { get; set; }

        [Required]
        public Guid ProductItemID { get; set; }

        public double? RentPrice { get; set; }

        public double? SalePrice { get; set; }

        public int? Quantity { get; set; }

        public string Status { get; set; } = null!;

        public List<string> ImagesUrls { get; set; } = null!;
    }
}

