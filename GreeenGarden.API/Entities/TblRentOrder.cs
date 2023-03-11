using System;
using System.Collections.Generic;

namespace GreeenGarden.API.Entities;

public partial class TblRentOrder
{
    public Guid Id { get; set; }

    public double? TransportFee { get; set; }

    public DateTime StartDateRent { get; set; }

    public DateTime EndDateRent { get; set; }

    public double? Deposit { get; set; }

    public double? TotalPrice { get; set; }

    public string? Status { get; set; }

    public double? RemainMoney { get; set; }

    public int? RewardPointGain { get; set; }

    public int? RewardPointUsed { get; set; }

    public Guid? ReferenceOrderId { get; set; }

    public virtual ICollection<TblRentOrderDetail> TblRentOrderDetails { get; } = new List<TblRentOrderDetail>();

    public virtual ICollection<TblTransaction> TblTransactions { get; } = new List<TblTransaction>();
}
