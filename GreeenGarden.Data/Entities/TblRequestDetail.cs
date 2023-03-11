using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblRequestDetail
{
    public Guid Id { get; set; }

    public string? TreeName { get; set; }

    public int? Quantity { get; set; }

    public string? Description { get; set; }

    public Guid RequestId { get; set; }

    public Guid? ServiceOrderId { get; set; }

    public double? Price { get; set; }

    public virtual TblRequest Request { get; set; } = null!;

    public virtual TblServiceOrder? ServiceOrder { get; set; }
}
