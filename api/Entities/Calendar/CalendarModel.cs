using Entities.Common;

namespace Entities.Calendar
{
    public class CalendarModel : BaseEntity<long>
    {

        public DateTime Date { get; set; }
        public string ShamsiDate { get; set; }
        public bool IsHoliday { get; set; }
        public string HolidayDesription { get; set; }
        public int Weekday { get; set; }
        public List<CalendarEvent> CalendarEvents { get; set; }
    }
}
