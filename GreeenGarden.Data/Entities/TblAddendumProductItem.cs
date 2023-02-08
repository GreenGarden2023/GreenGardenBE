using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblAddendumProductItem
{
    public Guid Id { get; set; }

    public double? ProductItemPrice { get; set; }

    public Guid ProductItemId { get; set; }

    public Guid AddendumId { get; set; }

    public virtual TblAddendum Addendum { get; set; } = null!;

    public virtual TblProductItem ProductItem { get; set; } = null!;
}
