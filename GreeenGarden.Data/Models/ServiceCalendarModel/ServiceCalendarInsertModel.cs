using System;
namespace GreeenGarden.Data.Models.ServiceCalendarModel
{
	public class ServiceCalendarInitModel
	{
		public Guid ServiceOrderId { get; set; }

		public DateTime ServiceDate { get; set; }

    }
	public class ServiceCalendarUpdateModel
    {
        public Guid ServiceCalendarId { get; set; }

        public DateTime? NextServiceDate { get; set; }

        public string? ReportFileURL { get; set; }

        public string? Sumary { get; set; }
    }
    public class ServiceCalendarInsertModel
    {
        public ServiceCalendarInitModel? CalendarInitial { get; set; }

        public ServiceCalendarUpdateModel? CalendarUpdate { get; set; }
    }
}

