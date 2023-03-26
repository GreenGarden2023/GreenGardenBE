namespace GreeenGarden.Data.Models.ServiceModel
{
    public class ServiceResModel
    {
        public Guid ID { get; set; }

        public string ServiceCode { get; set; }

        public Guid UserID { get; set; }

        public int UserCurrentPoint { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? Name { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public bool? IsTransport { get; set; }

        public double? TransportFee { get; set; }

        public int? RewardPointUsed { get; set; }

        public string? Status { get; set; }

        public Guid? TechnicianID { get; set; }

        public string? TechnicianName { get; set; }

        public List<ServiceDetailResModel> ServiceDetailList { get; set; }

    }

    public class ServiceDetailResModel
    {
        public Guid ID { get; set; }

        public Guid UserTreeID { get; set; }

        public Guid ServiceID { get; set; }

        public string? TreeName { get; set; }

        public string? Description { get; set; }

        public int? Quantity { get; set; }

        public double? ServicePrice { get; set; }

        public string? ManagerDescription { get; set; }

        public List<string> ImgUrls { get; set; }
    }
}

