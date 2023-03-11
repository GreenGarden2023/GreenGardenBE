using System;
using System.Collections.Generic;

namespace GreeenGarden.API.Entities;

public partial class TblRole
{
    public Guid Id { get; set; }

    public string RoleName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<TblUser> TblUsers { get; } = new List<TblUser>();
}
