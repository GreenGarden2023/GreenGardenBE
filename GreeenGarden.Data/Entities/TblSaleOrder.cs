using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblSaleOrder
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public DateTime? CreateDate { get; set; }

    public double? TotalPrice { get; set; }

    public double? TransportFee { get; set; }

    public double? Deposit { get; set; }

    public int? RewardPointGain { get; set; }

    public string? Status { get; set; }

    public int? RewardPointUsed { get; set; }

    public virtual ICollection<TblSaleOrderDetail> TblSaleOrderDetails { get; } = new List<TblSaleOrderDetail>();

    public virtual ICollection<TblTransaction> TblTransactions { get; } = new List<TblTransaction>();
}
