using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblService
{
    public Guid Id { get; set; }

    public string? ServiceCode { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Name { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? Status { get; set; }

    public Guid UserId { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? TechnicianId { get; set; }

    public string? TechnicianName { get; set; }

    public bool? IsTransport { get; set; }

    public double? TransportFee { get; set; }

    public int? RewardPointUsed { get; set; }

    public virtual ICollection<TblServiceDetail> TblServiceDetails { get; } = new List<TblServiceDetail>();

    public virtual ICollection<TblServiceOrder> TblServiceOrders { get; } = new List<TblServiceOrder>();

    public virtual TblUser User { get; set; } = null!;
}
