using Entities.Common;
using Entities.Users;

namespace Entities.Calendar
{
    public class CalendarEvent : BaseEntity<long>
    {
        public long CalendarId { get; private set; }
        public CalendarModel Calendar { get; private set; }

        public long UserId { get; private set; }
        public User User { get; private set; }

        public DateTime Date { get; private set; }

        public string Title { get; private set; } = default!;
        public string Description { get; private set; }
        public CalendarEventType Type { get; private set; }

        public static CalendarEvent Create(
            long calendarId,
            long userId,
            DateTime date,            
            string title,
            string? description,
            CalendarEventType type)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required.");

            return new CalendarEvent
            {
                CalendarId = calendarId,
                UserId = userId,
                Date = date.Date,       // normalize
                Title = title,
                Description = description,
                Type = type
            };
        }

        public void Update(string title, string? description, CalendarEventType type)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required.");

            Title = title;
            Description = description;
            Type = type;

        }
    }
    public enum CalendarEventType
    {
        Personal = 1,
        Work = 2,
        Meeting = 3,
        Reminder = 4,
        Other = 5
    }

}
