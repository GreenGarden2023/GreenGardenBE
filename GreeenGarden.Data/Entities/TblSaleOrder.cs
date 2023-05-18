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

    public double? RemainMoney { get; set; }

    public double? DiscountAmount { get; set; }

    public string? RecipientAddress { get; set; }

    public int? RecipientDistrict { get; set; }

    public string? RecipientPhone { get; set; }

    public string? RecipientName { get; set; }

    public string? OrderCode { get; set; }

    public bool? IsTransport { get; set; }

    public string? Description { get; set; }

    public Guid? CancelBy { get; set; }

    public string? CareGuideUrl { get; set; }

    public virtual TblDistrict? RecipientDistrictNavigation { get; set; }

    public virtual ICollection<TblSaleOrderDetail> TblSaleOrderDetails { get; } = new List<TblSaleOrderDetail>();

    public virtual ICollection<TblTransaction> TblTransactions { get; } = new List<TblTransaction>();

    public virtual TblUser User { get; set; } = null!;
}
