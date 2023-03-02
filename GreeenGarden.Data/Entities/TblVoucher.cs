namespace GreeenGarden.Data.Entities;

public partial class TblVoucher
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Type { get; set; }

    public double? Value { get; set; }

    public string? Unit { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<TblOrder> TblOrders { get; } = new List<TblOrder>();
}
