using GreeenGarden.Data.Models.ServiceModel;

namespace GreeenGarden.Data.Models.OrderModel
{
    public class ServiceOrderGetResModel
    {
        public Guid Id { get; set; }

        public string? OrderCode { get; set; }

        public Guid UserID { get; set; }

        public ServiceOrderTechnician? Technician { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ServiceStartDate { get; set; }

        public DateTime ServiceEndDate { get; set; }

        public double Deposit { get; set; }

        public double TotalPrice { get; set; }

        public double DiscountAmount { get; set; }

        public double RemainAmount { get; set; }

        public int RewardPointGain { get; set; }

        public int RewardPointUsed { get; set; }

        public double TransportFee { get; set; }

        public string? Status { get; set; }
        public string? CareGuide { get; set; }
        public string? Reason { get; set; }
        public Guid? CancelBy { get; set; }
        public string? NameCancelBy { get; set; } =null;

        public ServiceResModel? Service { get; set; }
    }

    public class ServiceOrderTechnician
    {
        public Guid TechnicianID { get; set; }

        public string? TechnicianUserName { get; set; }

        public string? TechnicianFullName { get; set; }

        public string? TechnicianAddress { get; set; }

        public string? TechnicianPhone { get; set; }

        public string? TechnicianMail { get; set; }
    }
}

