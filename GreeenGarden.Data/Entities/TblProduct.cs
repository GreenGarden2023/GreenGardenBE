namespace GreeenGarden.Data.Entities;

public partial class TblProduct
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public Guid? CategoryId { get; set; }

    public virtual TblCategory? Category { get; set; }

    public virtual ICollection<TblImage> TblImages { get; } = new List<TblImage>();

    public virtual ICollection<TblProductItem> TblProductItems { get; } = new List<TblProductItem>();
}
