using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblSubProduct
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Price { get; set; }

    public Guid? SizeId { get; set; }

    public Guid ProductId { get; set; }

    public virtual TblProduct Product { get; set; } = null!;

    public virtual TblSize? Size { get; set; }

    public virtual ICollection<TblProductItem> TblProductItems { get; } = new List<TblProductItem>();
}
