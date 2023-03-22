using System;
namespace GreeenGarden.Data.Models.ServiceModel
{
	public class ServiceInsertModel
	{
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? Name { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public bool? IsTransport { get; set; }

        public List<Guid> UserTreeIDList { get; set; }
    }
    public class ServiceStatusModel
    {
        public Guid ServiceID { get; set; }

        public string status { get; set; }
    }
}

