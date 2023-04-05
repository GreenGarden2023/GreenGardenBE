namespace GreeenGarden.Data.Entities;

public partial class TblRentOrderDetail
{
    public Guid Id { get; set; }

    public double? TotalPrice { get; set; }

    public int? Quantity { get; set; }

    public Guid RentOrderId { get; set; }

    public double? RentPricePerUnit { get; set; }

    public string? SizeName { get; set; }

    public string? ProductItemName { get; set; }

    public Guid? ProductItemDetailId { get; set; }

    public bool? FeedbackStatus { get; set; }

    public virtual TblProductItemDetail? ProductItemDetail { get; set; }

    public virtual TblRentOrder RentOrder { get; set; } = null!;

    public virtual ICollection<TblImage> TblImages { get; } = new List<TblImage>();
}
