using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblRequest
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Status { get; set; }

    public int NumberTree { get; set; }

    public DateTime CreateDate { get; set; }
}
