using System;
namespace GreeenGarden.Data.Models.ServiceModel
{
    public class ServiceUpdateModelManager
    {
        public Guid ServiceDetailID { get; set; }

        public int? Quantity { get; set; }

        public double? ServicePrice { get; set; }

        public string? ManagerDescription { get; set; }
    }
    public class ServiceAssignModelManager
    {
        public Guid ServiceID { get; set; }
        public Guid TechnicianID { get; set; }
    }
}

