namespace GreeenGarden.Data.Models.ServiceModel
{
    public class ServiceUpdateModelManager
    {
        public Guid ServiceID { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? Name { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public string? Rules { get; set; }

        public string IsTranSport { get; set; }

        public double? TransportFee { get; set; }

        public int? RewardPointUsed { get; set; }

    }
    public class ServiceDetailUpdateModelManager
    {
        public Guid ServiceDetailID { get; set; }

        public int? Quantity { get; set; }

        public double? ServicePrice { get; set; }

        public string? ManagerDescription { get; set; }
    }
    public class UpdateService
    {
        public ServiceUpdateModelManager? ServiceUpdate { get; set; }
        public List<ServiceDetailUpdateModelManager>? ServiceDetailUpdate { get; set; }
    }

    public class ServiceAssignModelManager
    {
        public Guid ServiceID { get; set; }
        public Guid TechnicianID { get; set; }
    }
}

