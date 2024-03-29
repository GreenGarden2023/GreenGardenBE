﻿using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblTakecareComboServiceDetail
{
    public Guid Id { get; set; }

    public Guid TakecareComboServiceId { get; set; }

    public Guid TakecareComboId { get; set; }

    public string? TakecareComboName { get; set; }

    public double? TakecareComboPrice { get; set; }

    public string? TakecareComboDescription { get; set; }

    public string? TakecareComboGuarantee { get; set; }

    public string? TakecareComboCareguide { get; set; }

    public virtual TblTakecareCombo TakecareCombo { get; set; } = null!;

    public virtual TblTakecareComboService TakecareComboService { get; set; } = null!;
}
