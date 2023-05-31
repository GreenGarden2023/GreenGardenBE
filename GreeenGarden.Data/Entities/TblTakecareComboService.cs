using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblTakecareComboService
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public Guid TakecareComboId { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string? Name { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public Guid UserId { get; set; }

    public Guid? TechnicianId { get; set; }

    public string? TechnicianName { get; set; }

    public int TreeQuantity { get; set; }

    public string Status { get; set; } = null!;

    public int NumberOfMonths { get; set; }

    public bool? IsAtShop { get; set; }

    public Guid? CancelBy { get; set; }

    public string? CancelReason { get; set; }

    public string? CareGuide { get; set; }

    public virtual TblUser? CancelByNavigation { get; set; }

    public virtual TblTakecareCombo TakecareCombo { get; set; } = null!;

    public virtual ICollection<TblTakecareComboOrder> TblTakecareComboOrders { get; } = new List<TblTakecareComboOrder>();

    public virtual ICollection<TblTakecareComboServiceDetail> TblTakecareComboServiceDetails { get; } = new List<TblTakecareComboServiceDetail>();

    public virtual TblUser? Technician { get; set; }

    public virtual TblUser User { get; set; } = null!;
}
