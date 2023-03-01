using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblCart
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public string? Status { get; set; }
    
    public bool? IsForRent { get; set; }

    public virtual ICollection<TblCartDetail> TblCartDetails { get; } = new List<TblCartDetail>();

    public virtual TblUser? User { get; set; }
}
