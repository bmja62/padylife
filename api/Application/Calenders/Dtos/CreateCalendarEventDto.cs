using Entities.Calendar;

namespace Application.Calenders.Dtos
{
    public record CreateCalendarEventDto(
     DateTime Date,                  
     string Title,
     string? Description,
     CalendarEventType Type
 );
}
