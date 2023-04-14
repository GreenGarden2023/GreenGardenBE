using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblShippingFee
{
    public Guid Id { get; set; }

    public int DistrictId { get; set; }

    public double FeeAmount { get; set; }

    public virtual TblDistrict District { get; set; } = null!;
}
