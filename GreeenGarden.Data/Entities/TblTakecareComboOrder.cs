using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblTakecareComboOrder
{
    public Guid Id { get; set; }

    public string OrderCode { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public DateTime ServiceStartDate { get; set; }

    public DateTime ServiceEndDate { get; set; }

    public double? Deposit { get; set; }

    public double TotalPrice { get; set; }

    public double RemainAmount { get; set; }

    public Guid? TechnicianId { get; set; }

    public Guid TakecareComboServiceId { get; set; }

    public Guid? UserId { get; set; }

    public string Status { get; set; } = null!;

    public string? Description { get; set; }

    public Guid? CancelBy { get; set; }

    public virtual TblUser? CancelByNavigation { get; set; }

    public virtual TblTakecareComboService TakecareComboService { get; set; } = null!;

    public virtual ICollection<TblComboServiceCalendar> TblComboServiceCalendars { get; } = new List<TblComboServiceCalendar>();

    public virtual ICollection<TblTransaction> TblTransactions { get; } = new List<TblTransaction>();

    public virtual TblUser? Technician { get; set; }

    public virtual TblUser? User { get; set; }
}
