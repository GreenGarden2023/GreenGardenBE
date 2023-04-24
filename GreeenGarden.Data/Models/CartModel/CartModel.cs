namespace GreeenGarden.Data.Models.CartModel
{
    public class CartModel
    {

    }
    public class productItemDetail
    {
        public Guid Id { get; set; }
        public Guid SizeId { get; set; }
        public Guid ProductItemId { get; set; }
        public double? RentPrice { get; set; }
        public double? SalePrice { get; set; }
        public int? Quantity { get; set; }
        public string? Content { get; set; }
        public double? TransportFee { get; set; }
        public string? Status { get; set; }
        public size size { get; set; } = new size();
        public List<string> imgUrl { get; set; } = new List<string>();
    }

    public class productItem
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public string? Content { get; set; }
        public string? Rule { get; set; }
    }


    public class size
    {
        public Guid id { get; set; }
        public string? sizeName { get; set; }
    }
    public class ItemRequest
    {
        public productItemDetail productItemDetail { get; set; } = new productItemDetail();
        public productItem? productItem { get; set; }
        public int? quantity { get; set; }
        public double? unitPrice { get; set; }
    }
    public class CartShowModel
    {
        public List<ItemRequest>? rentItems { get; set; }
        public double? totalRentPrice { get; set; }
        public List<ItemRequest>? saleItems { get; set; }
        public double? totalSalePrice { get; set; }
        public double? totalPrice { get; set; }
    }
    public class AddToCartModel
    {
        public List<ItemResponse>? rentItems { get; set; }
        public List<ItemResponse>? saleItems { get; set; }
        public string? status { get; set; }
    }
    public class ItemResponse
    {
        public Guid? productItemDetailID { get; set; }
        public int? quantity { get; set; }
    }
}
