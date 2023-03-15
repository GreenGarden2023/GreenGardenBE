using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblUserTree
{
    public Guid Id { get; set; }

    public string? TreeName { get; set; }

    public Guid? UserId { get; set; }

    public string? Description { get; set; }

    public int? Quantity { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<TblServiceUserTree> TblServiceUserTrees { get; } = new List<TblServiceUserTree>();
}
