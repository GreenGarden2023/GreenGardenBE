using System;
namespace GreeenGarden.Data.Models.ServiceCalendarModel
{
	public class ServiceCalendarResModel
	{
        public Guid Id { get; set; }

        public Guid ServiceOrderId { get; set; }

        public DateTime? ServiceDate { get; set; }

        public DateTime? NextServiceDate { get; set; }

        public string? ReportFileURL { get; set; }

        public string? Sumary { get; set; }

        public string? Status { get; set; }
    }
    public class ServiceCalendarUpdateResModel
    {
        public ServiceCalendarResModel PreviousCalendar { get; set; }

        public ServiceCalendarResModel NextCalendar { get; set; }
    }

}

