using Entities.Calendar;

namespace Application.Calenders.Dtos
{

public record CalendarEventResponseDto(
    long Id,
    DateTime Date,               
    string Title,
    string? Description,
    CalendarEventType Type,
    DateTime CreatedAt
);
}
