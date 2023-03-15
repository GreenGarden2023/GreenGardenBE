using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblService
{
    public Guid Id { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
    public string? Mail { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Status { get; set; }

    public string? Name { get; set; }

    public Guid UserId { get; set; }

    public virtual ICollection<TblServiceOrder> TblServiceOrders { get; } = new List<TblServiceOrder>();

    public virtual ICollection<TblServiceUserTree> TblServiceUserTrees { get; } = new List<TblServiceUserTree>();
}
