using System;
using System.Collections.Generic;

namespace GreeenGarden.API.Entities;

public partial class TblSaleOrderDetail
{
    public Guid Id { get; set; }

    public Guid? ProductItemDetailId { get; set; }

    public Guid? SaleOderId { get; set; }

    public double? ProductItemPrice { get; set; }

    public int? Quantity { get; set; }

    public virtual TblProductItemDetail? ProductItemDetail { get; set; }

    public virtual TblSaleOrder? SaleOder { get; set; }
}
