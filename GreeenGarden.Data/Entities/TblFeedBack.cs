using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblFeedBack
{
    public Guid Id { get; set; }

    public double? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid UserId { get; set; }

    public Guid ProductItemId { get; set; }

    public string? Status { get; set; }

    public Guid? OrderId { get; set; }

    public virtual TblProductItem ProductItem { get; set; } = null!;

    public virtual ICollection<TblImage> TblImages { get; } = new List<TblImage>();

    public virtual TblUser User { get; set; } = null!;
}
