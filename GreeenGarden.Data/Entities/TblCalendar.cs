using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblCalendar
{
    public Guid Id { get; set; }

    public Guid ServiceOrderId { get; set; }

    public string? Status { get; set; }

    public Guid UserId { get; set; }

    public virtual TblServiceOrder ServiceOrder { get; set; } = null!;

    public virtual ICollection<TblCalendarDetial> TblCalendarDetials { get; } = new List<TblCalendarDetial>();
}
