using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblCartDetail
{
    public Guid Id { get; set; }

    public Guid? SizeProductItemId { get; set; }

    public int? Quantity { get; set; }

    public Guid CartId { get; set; }

    public virtual TblCart Cart { get; set; } = null!;

    public virtual TblSizeProductItem? SizeProductItem { get; set; }
}
