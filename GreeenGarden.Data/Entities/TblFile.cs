using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblFile
{
    public Guid Id { get; set; }

    public string? FileUrl { get; set; }

    public Guid? ReportDetailId { get; set; }
}
