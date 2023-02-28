using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblSize
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<TblSizeProductItem> TblSizeProductItems { get; } = new List<TblSizeProductItem>();
}
