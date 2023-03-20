namespace GreeenGarden.Data.Entities;

public partial class TblServiceUserTree
{
    public Guid Id { get; set; }

    public Guid? UserTreeId { get; set; }

    public Guid? ServiceId { get; set; }

    public int? Quantity { get; set; }

    public double? Price { get; set; }

    public virtual TblService? Service { get; set; }

    public virtual TblUserTree? UserTree { get; set; }
}
