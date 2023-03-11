using System;
using System.Collections.Generic;

namespace GreeenGarden.API.Entities;

public partial class TblSize
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public bool? Type { get; set; }

    public virtual ICollection<TblProductItemDetail> TblProductItemDetails { get; } = new List<TblProductItemDetail>();
}
