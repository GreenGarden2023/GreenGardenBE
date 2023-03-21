using System;
namespace GreeenGarden.Data.Models.ServiceModel
{
	public class ServiceManagerUpdateModel
	{
        public Guid ServiceID { get; set; }

        public Guid TechnicianID { get; set; }

        List<ServiceDetailManagerUpdateModel> ServiceDetailUpdateList { get; set; }
    }
    public class ServiceDetailManagerUpdateModel
    {

        public Guid ServiceDetailID { get; set; }

        public int? Quantity { get; set; }

        public double? ServicePrice { get; set; }

        public string? ManagerDescription { get; set; }
    }
}

