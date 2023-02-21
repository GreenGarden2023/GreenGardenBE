namespace GreeenGarden.Data.Entities;

public partial class TblSize
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<TblProductItem> TblProductItems { get; } = new List<TblProductItem>();
}
