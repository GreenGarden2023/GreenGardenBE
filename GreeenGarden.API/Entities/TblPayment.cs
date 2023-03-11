using System;
using System.Collections.Generic;

namespace GreeenGarden.API.Entities;

public partial class TblPayment
{
    public Guid Id { get; set; }

    public string Status { get; set; } = null!;

    public string PaymentMethod { get; set; } = null!;

    public virtual ICollection<TblTransaction> TblTransactions { get; } = new List<TblTransaction>();
}
