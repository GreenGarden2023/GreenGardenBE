using GreeenGarden.Data.Models.FeedbackModel;
using GreeenGarden.Data.Models.ProductItemDetailModel;

namespace GreeenGarden.Data.Models.OrderModel
{
    public class RentOrderResModel
    {
        public Guid Id { get; set; }

        public bool? IsTransport { get; set; }

        public double? TransportFee { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime StartRentDate { get; set; }

        public DateTime EndRentDate { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UserId { get; set; }

        public double? Deposit { get; set; }

        public double? TotalPrice { get; set; }

        public string? Status { get; set; }

        public double? RemainMoney { get; set; }

        public int? RewardPointGain { get; set; }

        public int? RewardPointUsed { get; set; }

        public Guid? RentOrderGroupID { get; set; }

        public double? DiscountAmount { get; set; }

        public string? RecipientAddress { get; set; }

        public int? RecipientDistrict { get; set; }

        public string? RecipientPhone { get; set; }

        public string? RecipientName { get; set; }

        public string? OrderCode { get; set; }

        public List<RentOrderDetailResModel>? RentOrderDetailList { get; set; }
    }
    public class RentOrderDetailResModel
    {
        public Guid ID { get; set; }
        public ProductItemDetailResModel? ProductItemDetail { get; set; }
        public double? TotalPrice { get; set; }
        public int? Quantity { get; set; }
        public double? RentPricePerUnit { get; set; }
        public string? SizeName { get; set; }
        public string? ProductItemName { get; set; }
        public string? ImgURL { get; set; }
        public List<FeedbackOrderResModel>? FeedbackList { get; set; }
    }

    public class RentOrderByDateResModel
    {
        public Guid Id { get; set; }

        public bool? IsTransport { get; set; }

        public double? TransportFee { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime StartRentDate { get; set; }

        public DateTime EndRentDate { get; set; }

        public Guid? CreatedBy { get; set; }

        public Guid? UserId { get; set; }

        public double? Deposit { get; set; }

        public double? TotalPrice { get; set; }

        public string? Status { get; set; }

        public double? RemainMoney { get; set; }

        public int? RewardPointGain { get; set; }

        public int? RewardPointUsed { get; set; }

        public Guid? RentOrderGroupID { get; set; }

        public double? DiscountAmount { get; set; }

        public string? RecipientAddress { get; set; }

        public int? RecipientDistrict { get; set; }

        public string? RecipientPhone { get; set; }

        public string? RecipientName { get; set; }

        public string? OrderCode { get; set; }

        public List<RentOrderDetailResModel>? RentOrderDetailList { get; set; }
        public RentOrderGroupModel? RentOrderGroupModel { get; set; } = new RentOrderGroupModel();
    }
}


