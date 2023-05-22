using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblComboServiceCalendar
{
    public Guid Id { get; set; }

    public Guid TakecareComboOrderId { get; set; }

    public DateTime? ServiceDate { get; set; }

    public DateTime? NextServiceDate { get; set; }

    public string? Sumary { get; set; }

    public string? Status { get; set; }

    public virtual TblTakecareComboOrder TakecareComboOrder { get; set; } = null!;

    public virtual ICollection<TblImage> TblImages { get; } = new List<TblImage>();
}
