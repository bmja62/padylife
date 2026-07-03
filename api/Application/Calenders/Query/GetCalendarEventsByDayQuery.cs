using Application.Calenders.Dtos;
using Application.Cqrs.Queris;
using Common.Utilities;
using Data.Contracts;
using Entities.Calendar;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Calenders.Query
{
    public record GetCalendarEventsByDayQuery(DateTime Date)
      : IQuery<ServiceResult<List<CalendarEventResponseDto>>>;

    public class GetCalendarEventsByDayHandler
      : IQueryHandler<GetCalendarEventsByDayQuery, ServiceResult<List<CalendarEventResponseDto>>>
    {
        private readonly IRepository<CalendarEvent> _eventRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetCalendarEventsByDayHandler(
            IRepository<CalendarEvent> eventRepo,
            IHttpContextAccessor httpContextAccessor)
        {
            _eventRepo = eventRepo;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResult<List<CalendarEventResponseDto>>> Handle(
            GetCalendarEventsByDayQuery request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.GetUserId<long>();
            var targetDate = request.Date.Date;

            var list = await _eventRepo.TableNoTracking
                .Where(e => e.UserId == userId && e.Date == targetDate)
                .Select(e => new CalendarEventResponseDto(
                    e.Id,
                    e.Date,
                    e.Title,
                    e.Description,
                    e.Type,
                    e.CreatedAt
                ))
                .ToListAsync(cancellationToken);

            return ServiceResult.Ok(list);
        }
    }


}
