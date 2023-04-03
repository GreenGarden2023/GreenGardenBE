using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

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

    public Guid? RentOrderGroupId { get; set; }

    public double? DiscountAmount { get; set; }

    public Guid? UserId { get; set; }

    public string? RecipientAddress { get; set; }

    public int? RecipientDistrict { get; set; }

    public string? RecipientPhone { get; set; }

    public string? RecipientName { get; set; }

    public string? OrderCode { get; set; }

    public DateTime? CreateDate { get; set; }

    public bool? IsTransport { get; set; }

    public Guid CreatedBy { get; set; }

    public bool? FeedbackStatus { get; set; }

    public virtual TblUser CreatedByNavigation { get; set; } = null!;

    public virtual TblDistrict? RecipientDistrictNavigation { get; set; }

    public virtual TblRentOrderGroup? RentOrderGroup { get; set; }

    public virtual ICollection<TblRentOrderDetail> TblRentOrderDetails { get; } = new List<TblRentOrderDetail>();

    public virtual ICollection<TblTransaction> TblTransactions { get; } = new List<TblTransaction>();

    public virtual TblUser? User { get; set; }
}
