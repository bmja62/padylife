

namespace Application.Calenders.Dtos
{


    public record GetMonthCalendarResponseDto(
      int ShamsiYear,
      int ShamsiMonth,
      IReadOnlyList<MonthDayDto> Days,
      IReadOnlyList<string> MonthOccasions
  );

    public record MonthDayDto(
      int ShamsiDay,         
      int ShamsiMonth,       
      int MiladiDay,
      long DayId,
      int MiladiMonth,       
      int Weekday,           
      int WeekOfMonth,       
      bool IsHoliday,        
      List<string> Occasions,
          DateTime Date,                 
          IReadOnlyList<CalendarEventItemDto> Events,
      bool IsInMonth
  );
}
