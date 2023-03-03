namespace GreeenGarden.Data.Models.CartModel
{
    public class CartModel
    {

    }
    public class ItemRequest
    {
        public Guid? sizeProductItemID { get; set; }
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
