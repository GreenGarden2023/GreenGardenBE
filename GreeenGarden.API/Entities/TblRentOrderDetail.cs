﻿using System;
using System.Collections.Generic;

namespace GreeenGarden.API.Entities;

public partial class TblRentOrderDetail
{
    public Guid Id { get; set; }

    public double? ProductItemRentPrice { get; set; }

    public int? Quantity { get; set; }

    public Guid ProductItemDetailId { get; set; }

    public Guid RentOrderId { get; set; }

    public virtual TblProductItemDetail ProductItemDetail { get; set; } = null!;

    public virtual TblRentOrder RentOrder { get; set; } = null!;
}
