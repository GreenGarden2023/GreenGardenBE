﻿namespace GreeenGarden.Data.Entities;

public partial class TblProductItem
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public Guid ProductId { get; set; }

    public string Type { get; set; } = null!;

    public virtual TblProduct Product { get; set; } = null!;

    public virtual ICollection<TblAddendumProductItem> TblAddendumProductItems { get; } = new List<TblAddendumProductItem>();

    public virtual ICollection<TblFeedBack> TblFeedBacks { get; } = new List<TblFeedBack>();

    public virtual ICollection<TblSizeProductItem> TblSizeProductItems { get; } = new List<TblSizeProductItem>();
}
