namespace GreeenGarden.Data.Entities;

public partial class TblSizeProductItem
{
    public Guid Id { get; set; }
    public Guid SizeId { get; set; }

    public Guid ProductItemId { get; set; }

    public double? RentPrice { get; set; }

    public double? SalePrice { get; set; }

    public int? Quantity { get; set; }

    public string? Content { get; set; }

    public string Status { get; set; } = null!;

    public virtual TblProductItem ProductItem { get; set; } = null!;

    public virtual TblSize Size { get; set; } = null!;

    public virtual ICollection<TblCartDetail> TblCartDetails { get; } = new List<TblCartDetail>();
    public virtual ICollection<TblImage> TblImages { get; } = new List<TblImage>();
}
