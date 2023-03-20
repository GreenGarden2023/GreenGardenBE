namespace GreeenGarden.Data.Entities;

public partial class TblCalendarDetial
{
    public Guid Id { get; set; }

    public DateTime DateReport { get; set; }

    public Guid CalendarId { get; set; }

    public virtual TblCalendar Calendar { get; set; } = null!;
}
