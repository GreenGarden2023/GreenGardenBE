namespace GreeenGarden.Data.Entities;

public partial class TblSubProduct
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public double? Price { get; set; }

    public Guid? SizeId { get; set; }

    public Guid? ProductId { get; set; }

    public int? Quantity { get; set; }

    public double? MinPrice { get; set; }

    public double? MaxPrice { get; set; }

    public virtual TblProduct? Product { get; set; }

    public virtual TblSize? Size { get; set; }

    public virtual ICollection<TblProductItem> TblProductItems { get; } = new List<TblProductItem>();
}
