using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblServiceCalendar
{
    public Guid Id { get; set; }

    public Guid ServiceOrderId { get; set; }

    public DateTime? ServiceDate { get; set; }

    public DateTime? NextServiceDate { get; set; }

    public string? ReportFileUrl { get; set; }

    public string? Sumary { get; set; }

    public string? Status { get; set; }

    public virtual TblServiceOrder ServiceOrder { get; set; } = null!;
}
