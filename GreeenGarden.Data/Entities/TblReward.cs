using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblReward
{
    public Guid Id { get; set; }

    public int? CurrentPoint { get; set; }

    public int? Total { get; set; }

    public Guid? UserId { get; set; }

    public virtual TblUser? User { get; set; }
}
