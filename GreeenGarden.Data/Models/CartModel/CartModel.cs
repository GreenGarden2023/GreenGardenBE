using GreeenGarden.Data.Entities;

namespace GreeenGarden.Data.Models.CartModel
{
    public class CartModel
    {

    }
    public class sizeProductItem
    {
        public Guid Id { get; set; }
        public Guid SizeId { get; set; }
        public Guid ProductItemId { get; set; }
        public double? RentPrice { get; set; }
        public double? SalePrice { get; set; }
        public int? Quantity { get; set; }
        public string? Content { get; set; }
        public string Status { get; set; }
        public TblSize size { get; set; }
        public List<string> imgUrl { get; set; } = new List<string>();

    }
    public class ItemRequest
    {
        public sizeProductItem sizeProductItem { get; set; } = new sizeProductItem();
        public TblProductItem productItem { get; set; }
        public int? quantity { get; set; }
        public double? unitPrice { get; set; }
    }
    public class CartShowModel
    {
        public List<ItemRequest> rentItems { get; set; }
        public double? totalRentPrice { get; set; }
        public List<ItemRequest> saleItems { get; set; }
        public double? totalSalePrice { get; set; }
        public double? totalPrice { get; set; }
    }
    public class AddToCartModel
    {
        public List<ItemResponse> rentItems { get; set; }
        public List<ItemResponse> saleItems { get; set; } 
    }
    public class ItemResponse
    {
        public Guid? sizeProductItemID { get; set; }
        public int? quantity { get; set; }
    }
}
