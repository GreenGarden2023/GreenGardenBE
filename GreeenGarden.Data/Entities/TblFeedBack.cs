namespace GreeenGarden.Data.Entities;

public partial class TblFeedBack
{
    public Guid Id { get; set; }

    public double Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid UserId { get; set; }

    public string? Status { get; set; }

    public Guid? OrderId { get; set; }

    public Guid? ProductItemDetailId { get; set; }

    public DateTime? UpdateDate { get; set; }

    public virtual TblProductItemDetail? ProductItemDetail { get; set; }

    public virtual ICollection<TblImage> TblImages { get; } = new List<TblImage>();

    public virtual TblUser User { get; set; } = null!;
}
