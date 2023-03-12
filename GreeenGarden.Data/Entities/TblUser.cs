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

    public virtual ICollection<TblCart> TblCarts { get; } = new List<TblCart>();

    public virtual ICollection<TblEmailOtpcode> TblEmailOtpcodes { get; } = new List<TblEmailOtpcode>();

    public virtual ICollection<TblFeedBack> TblFeedBacks { get; } = new List<TblFeedBack>();

    public virtual ICollection<TblRentOrder> TblRentOrders { get; } = new List<TblRentOrder>();

    public virtual ICollection<TblReport> TblReports { get; } = new List<TblReport>();

    public virtual ICollection<TblRequest> TblRequests { get; } = new List<TblRequest>();

    public virtual ICollection<TblReward> TblRewards { get; } = new List<TblReward>();

    public virtual ICollection<TblSaleOrder> TblSaleOrders { get; } = new List<TblSaleOrder>();
}
