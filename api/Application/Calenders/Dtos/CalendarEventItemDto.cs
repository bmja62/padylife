using Entities.Calendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Calenders.Dtos
{
    public record CalendarEventItemDto(
        long Id,
        string Title,
        CalendarEventType Type,
        string? Description
    );

}
