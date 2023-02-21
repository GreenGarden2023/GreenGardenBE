namespace GreeenGarden.Data.Entities;

public partial class TblTransaction
{
    public Guid Id { get; set; }

    public string? Type { get; set; }

    public Guid? AddendumId { get; set; }

    public virtual TblAddendum? Addendum { get; set; }

    public virtual ICollection<TblPayment> TblPayments { get; } = new List<TblPayment>();
}
