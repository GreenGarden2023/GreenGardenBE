namespace GreeenGarden.Data.Entities;

public partial class TblAddendum
{
    public Guid Id { get; set; }

    public double? TransportFee { get; set; }

    public DateTime? StartDateRent { get; set; }

    public DateTime? EndDateRent { get; set; }

    public double? Deposit { get; set; }

    public double? ReducedMoney { get; set; }

    public double? TotalPrice { get; set; }

    public string? Status { get; set; }

    public Guid OrderId { get; set; }

    public double? RemainMoney { get; set; }

    public string? Address { get; set; }

    public virtual TblOrder Order { get; set; } = null!;

    public virtual ICollection<TblAddendumProductItem> TblAddendumProductItems { get; } = new List<TblAddendumProductItem>();

    public virtual ICollection<TblTransaction> TblTransactions { get; } = new List<TblTransaction>();
}
