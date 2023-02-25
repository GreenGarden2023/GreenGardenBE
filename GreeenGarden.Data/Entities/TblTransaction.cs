using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblTransaction
{
    public Guid Id { get; set; }

    public Guid? OrderId { get; set; }

    public Guid? AddendumId { get; set; }

    public double? Amount { get; set; }

    public string? Type { get; set; }

    public string? Status { get; set; }

    public DateTime? DatetimePaid { get; set; }

    public virtual TblAddendum? Addendum { get; set; }

    public virtual TblOrder? Order { get; set; }

    public virtual ICollection<TblPayment> TblPayments { get; } = new List<TblPayment>();
}
