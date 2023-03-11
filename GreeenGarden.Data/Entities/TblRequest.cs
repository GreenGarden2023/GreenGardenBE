using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblRequest
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Status { get; set; }

    public int NumberTree { get; set; }

    public DateTime CreateDate { get; set; }

    public Guid UserId { get; set; }

    public virtual ICollection<TblImage> TblImages { get; } = new List<TblImage>();

    public virtual ICollection<TblOrder> TblOrders { get; } = new List<TblOrder>();

    public virtual TblUser User { get; set; } = null!;
}
