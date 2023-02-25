using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblTransaction
{
    public Guid Id { get; set; }

    public double? Amount { get; set; }

    public Guid? PaymentId { get; set; }

    public DateTime? DatetimePay { get; set; }

    public string? Status { get; set; }

    public virtual TblPayment? Payment { get; set; }
}
