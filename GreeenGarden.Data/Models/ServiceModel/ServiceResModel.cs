using System;
namespace GreeenGarden.Data.Models.ServiceModel
{
	public class ServiceResModel
	{
        public Guid ID { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? Name { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? Status { get; set; }

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
    }
}

