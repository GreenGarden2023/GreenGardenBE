namespace GreeenGarden.Data.Models.OrderModel
{
    public class SaleOrderResModel
    {
        public Guid Id { get; set; }

        public double? TransportFee { get; set; }

        public DateTime CreateDate { get; set; }

        public double? Deposit { get; set; }

        public double? TotalPrice { get; set; }

        public string? Status { get; set; }

        public double? RemainMoney { get; set; }

        public int? RewardPointGain { get; set; }

        public int? RewardPointUsed { get; set; }

        public double? DiscountAmount { get; set; }

        public string? RecipientAddress { get; set; }

        public string? RecipientPhone { get; set; }

        public string? RecipientName { get; set; }

        public string? OrderCode { get; set; }

        public List<SaleOrderDetailResModel> RentOrderDetailList { get; set; }
    }
    public class SaleOrderDetailResModel
    {
        public Guid ID { get; set; }
        public Guid? ProductItemDetailID { get; set; }
        public string? ProductItemName { get; set; }
        public int? Quantity { get; set; }
        public double? SalePricePerUnit { get; set; }
        public double? TotalPrice { get; set; }
        public string? SizeName { get; set; }
        public string? ImgURL { get; set; }
    }
}


