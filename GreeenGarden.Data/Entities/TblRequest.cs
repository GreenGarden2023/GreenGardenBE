using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblRequest
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? TreeName { get; set; }

    public int? Quantity { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? Descripton { get; set; }

    public virtual ICollection<TblImage> TblImageReportNavigations { get; } = new List<TblImage>();

    public virtual ICollection<TblImage> TblImageRequests { get; } = new List<TblImage>();

    public virtual ICollection<TblServiceOrder> TblServiceOrders { get; } = new List<TblServiceOrder>();

    public virtual TblUser User { get; set; } = null!;
}
