using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblPayment
{
    public Guid Id { get; set; }

    public Guid? TransactionId { get; set; }

    public string? Status { get; set; }

    public string? PaymentMethod { get; set; }

    public virtual TblTransaction? Transaction { get; set; }
}
