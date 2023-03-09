namespace GreeenGarden.Data.Entities;

public partial class TblSize
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
<<<<<<< HEAD

    public bool? Type { get; set; }

=======

    public bool? Type { get; set; }

>>>>>>> df73b02 (Update dbcontext)
    public virtual ICollection<TblSizeProductItem> TblSizeProductItems { get; } = new List<TblSizeProductItem>();
}
