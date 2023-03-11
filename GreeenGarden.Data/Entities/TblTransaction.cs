namespace GreeenGarden.Data.Entities;

public partial class TblTransaction
{
    public Guid Id { get; set; }

    public Guid? RentOrderId { get; set; }

    public double? Amount { get; set; }

    public string? Type { get; set; }

    public string? Status { get; set; }

    public DateTime? DatetimePaid { get; set; }

    public Guid PaymentId { get; set; }

    public Guid? SaleOrderId { get; set; }

    public Guid? ServiceOrderId { get; set; }

    public virtual TblPayment Payment { get; set; } = null!;

    public virtual TblRentOrder? RentOrder { get; set; }

    public virtual TblSaleOrder? SaleOrder { get; set; }

    public virtual TblServiceOrder? ServiceOrder { get; set; }
}
