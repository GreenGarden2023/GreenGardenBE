using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblUser
{
    public Guid Id { get; set; }

    public string UserName { get; set; } = null!;

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string Favorite { get; set; } = null!;

    public string Mail { get; set; } = null!;

    public Guid RoleId { get; set; }

    public virtual TblRole Role { get; set; } = null!;

    public virtual ICollection<TblEmailOtpcode> TblEmailOtpcodes { get; } = new List<TblEmailOtpcode>();

    public virtual ICollection<TblFeedBack> TblFeedBacks { get; } = new List<TblFeedBack>();

    public virtual ICollection<TblOrder> TblOrders { get; } = new List<TblOrder>();
}
