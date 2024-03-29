﻿using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblProductItemDetail
{
    public Guid Id { get; set; }

    public Guid SizeId { get; set; }

    public Guid ProductItemId { get; set; }

    public double? RentPrice { get; set; }

    public double? SalePrice { get; set; }

    public int? Quantity { get; set; }

    public string Status { get; set; } = null!;

    public double? TransportFee { get; set; }

    public virtual TblProductItem ProductItem { get; set; } = null!;

    public virtual TblSize Size { get; set; } = null!;

    public virtual ICollection<TblCartDetail> TblCartDetails { get; } = new List<TblCartDetail>();

    public virtual ICollection<TblFeedBack> TblFeedBacks { get; } = new List<TblFeedBack>();

    public virtual ICollection<TblImage> TblImages { get; } = new List<TblImage>();

    public virtual ICollection<TblRentOrderDetail> TblRentOrderDetails { get; } = new List<TblRentOrderDetail>();
}
