using System;
using System.Collections.Generic;

namespace GreeenGarden.Data.Entities;

public partial class TblImage
{
    public Guid Id { get; set; }

    public string? ImageUrl { get; set; }

    public Guid? FeedbackId { get; set; }

    public Guid? ProductId { get; set; }

    public Guid? CategoryId { get; set; }

    public Guid? ProductItemDetailId { get; set; }

    public Guid? ProductItemId { get; set; }

    public Guid? ReportId { get; set; }

    public Guid? UserTreeId { get; set; }

    public Guid? RentOrderDetailId { get; set; }

    public Guid? SaleOrderDetailId { get; set; }

    public Guid? ServiceDetailId { get; set; }

    public virtual TblCategory? Category { get; set; }

    public virtual TblFeedBack? Feedback { get; set; }

    public virtual TblProduct? Product { get; set; }

    public virtual TblProductItem? ProductItem { get; set; }

    public virtual TblProductItemDetail? ProductItemDetail { get; set; }

    public virtual TblRentOrderDetail? RentOrderDetail { get; set; }

    public virtual TblReport? Report { get; set; }

    public virtual TblSaleOrderDetail? SaleOrderDetail { get; set; }

    public virtual TblServiceDetail? ServiceDetail { get; set; }
}
