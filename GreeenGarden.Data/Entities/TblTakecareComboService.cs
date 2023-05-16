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

    public virtual TblTakecareCombo TakecareCombo { get; set; } = null!;

    public virtual TblUser? Technician { get; set; }

    public virtual TblUser User { get; set; } = null!;
}
