using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblShippingFee
{
    public int Id { get; set; }

    public string District { get; set; } = null!;

    public double FeeAmount { get; set; }
}
