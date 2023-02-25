using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblPayment
{
    public Guid Id { get; set; }

    public string? PaymentMethod { get; set; }

    public Guid? OrderId { get; set; }

    public Guid? AddendumId { get; set; }

    public double? Amount { get; set; }

    public string? Type { get; set; }

    public string? Status { get; set; }

    public virtual TblAddendum? Addendum { get; set; }

    public virtual TblOrder? Order { get; set; }

    public virtual ICollection<TblTransaction> TblTransactions { get; } = new List<TblTransaction>();
}
