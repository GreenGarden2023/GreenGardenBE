using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblTakecareComboServiceDetail
{
    public Guid Id { get; set; }

    public Guid TakecareComboServiceId { get; set; }

    public string? TakecareComboName { get; set; }

    public double? TakecareComboPrice { get; set; }

    public string? TakecareComboDescription { get; set; }

    public string? TakecareComboGuarantee { get; set; }

    public virtual TblTakecareComboService TakecareComboService { get; set; } = null!;
}
