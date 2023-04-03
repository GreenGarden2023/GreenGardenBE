using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblSaleOrderDetail
{
    public Guid Id { get; set; }

    public Guid SaleOderId { get; set; }

    public Guid? ProductItemDetailId { get; set; }

    public double? TotalPrice { get; set; }

    public int? Quantity { get; set; }

    public double? SalePricePerUnit { get; set; }

    public string? SizeName { get; set; }

    public string? ProductItemName { get; set; }

    public virtual TblSaleOrder SaleOder { get; set; } = null!;

    public virtual ICollection<TblImage> TblImages { get; } = new List<TblImage>();
}
