using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblTakecareCombo
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public double Price { get; set; }

    public string? Description { get; set; }

    public string? Guarantee { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<TblTakecareComboServiceDetail> TblTakecareComboServiceDetails { get; } = new List<TblTakecareComboServiceDetail>();

    public virtual ICollection<TblTakecareComboService> TblTakecareComboServices { get; } = new List<TblTakecareComboService>();
}
