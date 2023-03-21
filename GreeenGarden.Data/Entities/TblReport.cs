﻿using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblReport
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? Sumary { get; set; }

    public Guid ServiceOrderId { get; set; }

    public virtual TblServiceOrder ServiceOrder { get; set; } = null!;

    public virtual ICollection<TblFile> TblFiles { get; } = new List<TblFile>();

    public virtual ICollection<TblImage> TblImages { get; } = new List<TblImage>();
}
