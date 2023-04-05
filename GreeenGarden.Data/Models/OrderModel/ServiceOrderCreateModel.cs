namespace GreeenGarden.Data.Models.OrderModel
{
    public class ServiceOrderCreateModel
    {
        public Guid ServiceId { get; set; }

    }

    public class ServiceOrderCreateResModel
    {
        public Guid Id { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ServiceStartDate { get; set; }

        public DateTime ServiceEndDate { get; set; }

        public double Deposit { get; set; }

        public double TotalPrice { get; set; }

        public double DiscountAmount { get; set; }

        public double RemainAmount { get; set; }

        public int RewardPointGain { get; set; }

        public int RewardPointUsed { get; set; }

        public Guid TechnicianID { get; set; }

        public Guid ServiceID { get; set; }

        public Guid UserID { get; set; }

        public double TransportFee { get; set; }

        public string? Status { get; set; }

    }
}

