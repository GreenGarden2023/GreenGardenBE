﻿using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblServiceOrder
{
    public Guid Id { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime? ServiceStartDate { get; set; }

    public DateTime? ServiceEndDate { get; set; }

    public double? Deposit { get; set; }

    public double? TotalPrice { get; set; }

    public string Status { get; set; } = null!;

    public int? RewardPointGain { get; set; }

    public int? RewardPointUsed { get; set; }

    public Guid TechnicianId { get; set; }

    public Guid ServiceId { get; set; }

    public double? Incurred { get; set; }

    public string? Description { get; set; }

    public Guid UserId { get; set; }

    public double? TransportFee { get; set; }

    public virtual TblService Service { get; set; } = null!;

    public virtual ICollection<TblCalendar> TblCalendars { get; } = new List<TblCalendar>();

    public virtual ICollection<TblReport> TblReports { get; } = new List<TblReport>();

    public virtual ICollection<TblTransaction> TblTransactions { get; } = new List<TblTransaction>();
}
