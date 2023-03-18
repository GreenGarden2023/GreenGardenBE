using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblRentOrderGroup
{
    public Guid Id { get; set; }

    public double? GroupTotalAmount { get; set; }

    public int? NumberOfOrders { get; set; }

    public Guid? UserId { get; set; }

    public DateTime? CreateDate { get; set; }

    public virtual ICollection<TblRentOrder> TblRentOrders { get; } = new List<TblRentOrder>();
}
