namespace GreeenGarden.Data.Models.ProductModel
{
    public class ProductModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public Guid CategoryId { get; set; }
        public string? ImgUrl { get; set; }
        public bool? IsForSale { get; set; }
        public bool? IsForRent { get; set; }
    }

    public class ProductSearchModel
    {
        public Guid? categoryID { get; set; }
    }
    public class ProductItemSearchModel
    {
        public Guid? productID { get; set; }
    }


}
