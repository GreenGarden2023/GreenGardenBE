using System;
using System.Collections.Generic;

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

    public Guid? RewardId { get; set; }

    public Guid? RequestId { get; set; }

    public virtual TblRequest? Request { get; set; }

    public virtual TblReward? Reward { get; set; }

    public virtual ICollection<TblAddendum> TblAddenda { get; } = new List<TblAddendum>();

    public virtual ICollection<TblTransaction> TblTransactions { get; } = new List<TblTransaction>();

    public virtual TblUser User { get; set; } = null!;
}
