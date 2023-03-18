using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblCalender
{
    public Guid Id { get; set; }

    public Guid ServiceOrderId { get; set; }

    public string? Status { get; set; }

    public Guid UserId { get; set; }
}
