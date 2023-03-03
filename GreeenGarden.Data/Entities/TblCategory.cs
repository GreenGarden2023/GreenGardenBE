using System;
using System.Collections.Generic;
namespace GreeenGarden.Data.Entities;

public partial class TblCategory
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Status { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<TblImage> TblImages { get; } = new List<TblImage>();

    public virtual ICollection<TblProduct> TblProducts { get; } = new List<TblProduct>();
}
