using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblTransaction
{
    public Guid Id { get; set; }

    public string? Type { get; set; }

    public Guid? PaymentId { get; set; }

    public virtual TblPayment? Payment { get; set; }
}
