using System;
namespace GreeenGarden.Data.Models.ServiceCalendarModel
{
		public class GetServiceCalendarsByUser
		{
			public Guid UserID { get; set; }

			public DateTime StartDate { get; set; }

            public DateTime EndDate { get; set; }

        }
		public class GetServiceCalendarsByTechnician
		{
            public Guid TechnicianID { get; set; }

            public DateTime Date { get; set; }
        }
}

