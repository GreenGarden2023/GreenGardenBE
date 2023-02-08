using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblProductItem
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public double? Price { get; set; }

    public string? Status { get; set; }

    public string? Description { get; set; }

    public Guid? SubProductId { get; set; }

    public virtual TblSubProduct? SubProduct { get; set; }

    public virtual ICollection<TblAddendumProductItem> TblAddendumProductItems { get; } = new List<TblAddendumProductItem>();

    public virtual ICollection<TblFeedBack> TblFeedBacks { get; } = new List<TblFeedBack>();

    public virtual ICollection<TblImage> TblImages { get; } = new List<TblImage>();
}
