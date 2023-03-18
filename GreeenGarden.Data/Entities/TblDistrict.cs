using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblDistrict
{
    public int Id { get; set; }

    public string DistrictName { get; set; } = null!;

    public virtual ICollection<TblUser> TblUsers { get; } = new List<TblUser>();
}
