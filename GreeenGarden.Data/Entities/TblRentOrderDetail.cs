namespace GreeenGarden.Data.Entities;

public partial class TblRentOrderDetail
{
    public Guid Id { get; set; }

    public double? ProductItemRentPrice { get; set; }

    public int? Quantity { get; set; }

    public Guid ProductItemId { get; set; }

    public Guid RentOrderId { get; set; }
}
