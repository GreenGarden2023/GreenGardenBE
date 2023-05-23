using System;
namespace GreeenGarden.Data.Models.TakecareComboCalendarModel
{


        public class TakecareComboCalendarInitModel
        {
            public Guid ServiceOrderId { get; set; }

            public string ServiceDate { get; set; }

        }
        public class TakecareComboCalendarUpdateModel
        {
            public Guid ServiceCalendarId { get; set; }

            public string? NextServiceDate { get; set; }

            public string? Sumary { get; set; }

            public List<string>? Images { get; set; }
        }
        public class TakecareComboCalendarInsertModel
        {
            public TakecareComboCalendarInitModel? CalendarInitial { get; set; }

            public TakecareComboCalendarUpdateModel? CalendarUpdate { get; set; }
        }


    public class ComboServiceCalendarResModel
    {
        public Guid Id { get; set; }

        public Guid ServiceOrderId { get; set; }

        public DateTime? ServiceDate { get; set; }

        public DateTime? NextServiceDate { get; set; }

        public string? Sumary { get; set; }

        public string? Status { get; set; }

        public List<string>? Images { get; set; }
    }
    public class ComboServiceCalendarUpdateResModel
    {
        public ComboServiceCalendarResModel? PreviousCalendar { get; set; }

        public ComboServiceCalendarResModel? NextCalendar { get; set; }
    }


    public class ComboServiceCalendarGetModel
    {
        public DateTime Date { get; set; }

        public Guid TechnicianId { get; set; }

        public int CalendarQuantity { get; set; }

        public List<ComboServiceCalendarResModel>? CalendarList { get; set; }
    }

    public class ComboServiceCalendarUserResModel
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

    public class ComboServiceCalendarUserGetModel
    {
        public DateTime Date { get; set; }

        public int CalendarQuantity { get; set; }

        public List<ComboServiceCalendarUserResModel>? CalendarList { get; set; }
    }

    public class GetComboServiceCalendarsByUser
    {
        public Guid UserID { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

    }
    public class GetComboServiceCalendarsByTechnician
    {
        public Guid TechnicianID { get; set; }

        public DateTime Date { get; set; }
    }
}

