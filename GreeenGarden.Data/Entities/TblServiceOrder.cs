using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblServiceOrder
{
    public Guid Id { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ServiceStartDate { get; set; }

    public DateTime? ServiceEndDate { get; set; }

    public double? Deposit { get; set; }

    public double? TotalPrice { get; set; }

    public string? Status { get; set; }

    public int? RewardPointGain { get; set; }

    public int? RewardPointUsed { get; set; }

    public Guid RequestId { get; set; }

    public Guid TechnicianId { get; set; }

    public virtual TblRequest Request { get; set; } = null!;

    public virtual ICollection<TblRequestDetail> TblRequestDetails { get; } = new List<TblRequestDetail>();

    public virtual ICollection<TblTransaction> TblTransactions { get; } = new List<TblTransaction>();
}
