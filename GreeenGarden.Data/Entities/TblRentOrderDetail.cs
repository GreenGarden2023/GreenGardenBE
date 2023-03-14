﻿using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblRentOrderDetail
{
    public Guid Id { get; set; }

    public double? TotalPrice { get; set; }

    public int? Quantity { get; set; }

    public Guid RentOrderId { get; set; }

    public double? RentPricePerUnit { get; set; }

    public string? SizeName { get; set; }

    public string? ProductItemName { get; set; }

    public virtual TblRentOrder RentOrder { get; set; } = null!;
}
