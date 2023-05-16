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

    public int? DistrictId { get; set; }

    public string? Phone { get; set; }

    public string? Favorite { get; set; }

    public string Mail { get; set; } = null!;

    public Guid RoleId { get; set; }

    public string? Status { get; set; }

    public virtual TblDistrict? District { get; set; }

    public virtual TblRole Role { get; set; } = null!;

    public virtual ICollection<TblCart> TblCarts { get; } = new List<TblCart>();

    public virtual ICollection<TblEmailOtpcode> TblEmailOtpcodes { get; } = new List<TblEmailOtpcode>();

    public virtual ICollection<TblFeedBack> TblFeedBacks { get; } = new List<TblFeedBack>();

    public virtual ICollection<TblRentOrder> TblRentOrderCreatedByNavigations { get; } = new List<TblRentOrder>();

    public virtual ICollection<TblRentOrder> TblRentOrderUsers { get; } = new List<TblRentOrder>();

    public virtual ICollection<TblReward> TblRewards { get; } = new List<TblReward>();

    public virtual ICollection<TblSaleOrder> TblSaleOrders { get; } = new List<TblSaleOrder>();

    public virtual ICollection<TblService> TblServices { get; } = new List<TblService>();
}
