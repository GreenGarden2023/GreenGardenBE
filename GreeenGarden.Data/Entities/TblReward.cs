using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblReward
{
    public Guid Id { get; set; }

    public int? CurrentPoint { get; set; }

    public double? Total { get; set; }

    public string? Status { get; set; }

    public Guid? UserId { get; set; }

    public virtual ICollection<TblOrder> TblOrders { get; } = new List<TblOrder>();

    public virtual TblUser? User { get; set; }
}
