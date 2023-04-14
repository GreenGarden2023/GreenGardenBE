using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblServiceDetail
{
    public Guid Id { get; set; }

    public Guid? UserTreeId { get; set; }

    public Guid? ServiceId { get; set; }

    public string? TreeName { get; set; }

    public string? Desciption { get; set; }

    public int? Quantity { get; set; }

    public double? ServicePrice { get; set; }

    public string? ManagerDescription { get; set; }

    public virtual TblService? Service { get; set; }

    public virtual ICollection<TblImage> TblImages { get; } = new List<TblImage>();

    public virtual TblUserTree? UserTree { get; set; }
}
