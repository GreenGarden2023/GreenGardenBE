using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblProductItem
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public Guid ProductId { get; set; }

    public string Type { get; set; } = null!;

    public string? Content { get; set; }

    public string? Rule { get; set; }

    public string? CareGuide { get; set; }

    public virtual TblProduct Product { get; set; } = null!;

    public virtual ICollection<TblImage> TblImages { get; } = new List<TblImage>();

    public virtual ICollection<TblProductItemDetail> TblProductItemDetails { get; } = new List<TblProductItemDetail>();
}
