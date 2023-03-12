using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblRequest
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public DateTime? CreateDate { get; set; }
    public string? Status { get; set; }

    public virtual ICollection<TblImage> TblImages { get; } = new List<TblImage>();
    public virtual ICollection<TblRequestDetail> TblRequestDetails { get; } = new List<TblRequestDetail>();

    public virtual ICollection<TblServiceOrder> TblServiceOrders { get; } = new List<TblServiceOrder>();

    public virtual TblUser User { get; set; } = null!;
}
