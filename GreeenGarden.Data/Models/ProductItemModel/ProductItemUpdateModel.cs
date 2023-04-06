namespace GreeenGarden.Data.Models.ProductItemModel
{
    public class ProductItemUpdateModel
    {
    }
    public class ProductItemDetailUpdateStatusModel
    {
        public Guid ProductItemDetailId { get; set; }
        public string? Status { get; set; }
    }
}
