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
        public double? totalPrice { get; set; }
        public bool? isForRent { get; set; }
        public List<ItemRequest> items { get; set; }
    }
    public class AddToCartModel
    {
        public bool? isForRent { get; set; }
        public List<ItemResponse> items { get; set; }
    }
    public class ItemResponse
    {
        public Guid? sizeProductItemID { get; set; }
        public int? quantity { get; set; }
    }
}
