﻿namespace GreeenGarden.Data.Models.ServiceCalendarModel
{
    public class ServiceCalendarResModel
    {
        public Guid Id { get; set; }

        public Guid ServiceOrderId { get; set; }

        public DateTime? ServiceDate { get; set; }

        public DateTime? NextServiceDate { get; set; }

        public string? Sumary { get; set; }

        public string? Status { get; set; }

        public List<string>? Images { get; set; }
    }
    public class ServiceCalendarTodayResModel
    {
        public Guid Id { get; set; }

        public Guid ServiceOrderId { get; set; }
        public string? ServiceOrderCode { get; set; }

        public DateTime? ServiceDate { get; set; }

        public DateTime? NextServiceDate { get; set; }

        public string? Sumary { get; set; }

        public string? Status { get; set; }

        public List<string>? Images { get; set; }
    }
    public class objTechResModel
    {
        public Guid TechnicianId { get; set; }
        public string? TechnicianName { get; set; }
        public string? TechnicianMail { get; set; }
        public string? Phone { get; set; }
    }
    public class ServiceCalendarUpdateResModel
    {
        public ServiceCalendarResModel? PreviousCalendar { get; set; }

        public ServiceCalendarResModel? NextCalendar { get; set; }
    }


    public class ServiceCalendarGetModel
    {
        public DateTime Date { get; set; }

        public Guid TechnicianId { get; set; }

        public int CalendarQuantity { get; set; }

        public List<ServiceCalendarResModel>? CalendarList { get; set; }
    }

    public class ServiceCalendarUserResModel
    {
        public Guid Id { get; set; }

        public Guid ServiceOrderId { get; set; }

        public Guid TechnicianId { get; set; }

        public DateTime? ServiceDate { get; set; }

        public DateTime? NextServiceDate { get; set; }

        public string? Sumary { get; set; }

        public string? Status { get; set; }

        public List<string>? Images { get; set; }
    }

    public class ServiceCalendarUserGetModel
    {
        public DateTime Date { get; set; }

        public int CalendarQuantity { get; set; }

        public List<ServiceCalendarUserResModel>? CalendarList { get; set; }
    }

    public class CarlendarToDayResModel
    {
        public Guid TechnicianId { get; set; }
        public string TechnicianName { get; set; }
        public string TechnicianMail { get; set; }
        public List<ServiceCalendarTodayResModel> listServiceCarlendar { get; set; }
        public List<ServiceCalendarTodayResModel> listComboCarlendar { get; set; }
    }

}

