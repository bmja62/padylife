using Application.Calenders.Dtos;
using Application.Cqrs.Queris;
using Common.Utilities;
using Data.Contracts;
using Entities.Calendar;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;

using System.Runtime.CompilerServices;


namespace Application.Calenders.Query
{

    public record GetMonthCalendarQuery(GetMonthCalendarRequestDto Input)
      : IQuery<ServiceResult<GetMonthCalendarResponseDto>>;

    public class GetMonthCalendarHandler
      : IQueryHandler<GetMonthCalendarQuery, ServiceResult<GetMonthCalendarResponseDto>>
    {
        private readonly IRepository<CalendarModel> _calendarRepo;
        private readonly IRepository<CalendarEvent> _eventRepo;
        private readonly IHttpContextAccessor _httpContext;

        public GetMonthCalendarHandler(
            IRepository<CalendarModel> calendarRepo,
            IRepository<CalendarEvent> eventRepo,
            IHttpContextAccessor httpContext)
        {
            _calendarRepo = calendarRepo;
            _eventRepo = eventRepo;
            _httpContext = httpContext;
        }

        public async Task<ServiceResult<GetMonthCalendarResponseDto>> Handle(
            GetMonthCalendarQuery request, CancellationToken cancellationToken)
        {
            var y = request.Input.ShamsiYear;
            var m = request.Input.ShamsiMonth;

            string firstDayKey = $"{y:0000}/{m:00}/01";

            // 1) روز اول ماه
            var firstDay = await _calendarRepo.TableNoTracking
                .Where(c => c.ShamsiDate == firstDayKey)
                .Select(c => new { c.Date, c.Weekday })
                .FirstOrDefaultAsync(cancellationToken);

            if (firstDay is null)
                return ServiceResult.BadRequest<GetMonthCalendarResponseDto>(
                    $"Calendar data not found for {firstDayKey}. Seed the calendar first.");

            // 2) محدوده‌ی 35 روز از شنبه‌ی همان هفته
            var gridStart = firstDay.Date.Date.AddDays(-(firstDay.Weekday - 1));
            var gridEnd = gridStart.AddDays(34);

            // 3) خواندن 35 روز
            var rangeList = await _calendarRepo.TableNoTracking
                .Where(c => c.Date >= gridStart && c.Date <= gridEnd)
                .OrderBy(c => c.Date)
                .Select(c => new CalendarRangeRow(
                    c.Id, c.Date, c.ShamsiDate, c.IsHoliday, c.HolidayDesription, c.Weekday))
                .ToListAsync(cancellationToken);

            if (rangeList.Count != 35)
                return ServiceResult.BadRequest<GetMonthCalendarResponseDto>(
                    $"Calendar range incomplete for {y:0000}/{m:00}. Expected 35, got {rangeList.Count}. Seed missing days.");

            var principal = _httpContext.HttpContext?.User;
            long? userId = (principal?.Identity?.IsAuthenticated == true)
                ? principal!.Identity!.GetUserId<long>()
                : null;

            Dictionary<long, List<CalendarEventItemDto>> eventsMap = new(35);
            if (userId.HasValue)
            {
                var calIds = new long[35];
                for (int i = 0; i < 35; i++) calIds[i] = rangeList[i].Id;

                var evs = await _eventRepo.TableNoTracking
                    .Where(e => calIds.Contains(e.CalendarId) && e.UserId == userId.Value)
                    .Select(e => new
                    {
                        e.CalendarId,
                        Item = new CalendarEventItemDto(e.Id, e.Title, e.Type, e.Description)
                    })
                    .ToListAsync(cancellationToken);

                foreach (var g in evs.GroupBy(x => x.CalendarId))
                {
                    var list = new List<CalendarEventItemDto>(g.Count());
                    foreach (var x in g) list.Add(x.Item);
                    eventsMap[g.Key] = list;
                }
            }

            var monthPrefix = $"{y:0000}/{m:00}/";

            var days = new List<MonthDayDto>(35);
            for (int i = 0; i < 35; i++)
            {
                var r = rangeList[i];

                bool isInMonth = r.ShamsiDate.StartsWith(monthPrefix, StringComparison.Ordinal);

                int shamsiDay = ParseDayFast(r.ShamsiDate);

                List<string> occ = isInMonth ? SplitOccasionsLight(r.HolidayDesription)
                                             : new List<string>(0);

                int weekOfMonth = ((int)(r.Date.Date - gridStart).TotalDays) / 7 + 1;

                eventsMap.TryGetValue(r.Id, out var dayEvents);
                dayEvents ??= new List<CalendarEventItemDto>(0);

                days.Add(new MonthDayDto(
                    Date: r.Date,
                    ShamsiDay: shamsiDay,
                    DayId:r.Id,
                    ShamsiMonth: m,
                    MiladiDay: r.Date.Day,
                    MiladiMonth: r.Date.Month,
                    Weekday: r.Weekday,
                    WeekOfMonth: weekOfMonth,
                    IsHoliday: r.IsHoliday,
                    Occasions: occ,
                    IsInMonth: isInMonth,
                    Events: dayEvents
                ));
            }

            var monthOccasions = days
                .Where(d => d.IsInMonth && d.Occasions.Count > 0)
                .SelectMany(d => d.Occasions)
                .Distinct()
                .ToList();

            var response = new GetMonthCalendarResponseDto(y, m, days, monthOccasions);
            return ServiceResult<GetMonthCalendarResponseDto>.Ok(response);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ParseDayFast(string shamsi /* yyyy/MM/dd */)
        {
            int d1 = shamsi[8] - '0';
            int d2 = shamsi[9] - '0';
            return d1 * 10 + d2;
        }

        private static List<string> SplitOccasionsLight(string? desc)
        {
            if (string.IsNullOrWhiteSpace(desc)) return new List<string>(0);
            var parts = desc.Split(new[] { '/', '،' },
                StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            return parts.Length == 0 ? new List<string>(0) : new List<string>(parts);
        }

        private readonly record struct CalendarRangeRow(
            long Id,
            DateTime Date,
            string ShamsiDate,
            bool IsHoliday,
            string? HolidayDesription,
            int Weekday);
    }



}
