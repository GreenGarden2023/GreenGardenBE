using System;
using System.Collections.Generic;
namespace GreeenGarden.Data.Entities;

public partial class TblEmailOtpcode
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string Optcode { get; set; } = null!;

    public virtual TblUser EmailNavigation { get; set; } = null!;
}
