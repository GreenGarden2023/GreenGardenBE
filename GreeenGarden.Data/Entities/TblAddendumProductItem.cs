namespace GreeenGarden.Data.Entities;

public partial class TblAddendumProductItem
{
    public Guid Id { get; set; }

    public double? SizeProductItemPrice { get; set; }

    public int? Quantity { get; set; }

    public Guid SizeProductItemId { get; set; }

    public Guid AddendumId { get; set; }

    public virtual TblAddendum Addendum { get; set; } = null!;

    public virtual TblSizeProductItem SizeProductItem { get; set; } = null!;
}
