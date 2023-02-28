using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblSizeProductItem
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public Guid? SizeId { get; set; }

    public Guid? ProductItemId { get; set; }

    public double? Price { get; set; }

    public virtual TblProductItem? ProductItem { get; set; }

    public virtual TblSize? Size { get; set; }
}
