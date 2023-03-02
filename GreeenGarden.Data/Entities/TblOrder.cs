namespace GreeenGarden.Data.Entities;

public partial class TblOrder
{
    public Guid Id { get; set; }

    public double? TotalPrice { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? Status { get; set; }

    public Guid UserId { get; set; }

    public Guid? VoucherId { get; set; }

    public bool? IsForRent { get; set; }

    public virtual ICollection<TblAddendum> TblAddenda { get; } = new List<TblAddendum>();

    public virtual ICollection<TblTransaction> TblTransactions { get; } = new List<TblTransaction>();

    public virtual TblUser User { get; set; } = null!;

    public virtual TblVoucher? Voucher { get; set; }
}
