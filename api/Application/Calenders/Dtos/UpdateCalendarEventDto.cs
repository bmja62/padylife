using Entities.Calendar;


namespace Application.Calenders.Dtos
{
    public record UpdateCalendarEventDto(
     long Id,
     string Title,
     string? Description,
     CalendarEventType Type
 );
}
