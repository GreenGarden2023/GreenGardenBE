using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblReport
{
    public Guid Id { get; set; }

    public int? ReportName { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<TblReportDetail> TblReportDetails { get; } = new List<TblReportDetail>();
}
