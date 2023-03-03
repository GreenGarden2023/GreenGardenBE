using System;
using System.Collections.Generic;
namespace GreeenGarden.Data.Entities;

public partial class TblReportDetail
{
    public Guid Id { get; set; }

    public Guid ReportId { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? Summary { get; set; }

    public virtual TblReport Report { get; set; } = null!;
}
