using Application.Calenders.Dtos;
using Application.Cqrs.Commands;
using Common.Utilities;
using Data.Contracts;
using Entities.Calendar;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Calenders.Command
{
    public record CreateCalendarEventCommand(CreateCalendarEventDto Input)
   : ICommand<ServiceResult<CalendarEventResponseDto>>;

    public class CreateCalendarEventCommandHandler
      : ICommandHandler<CreateCalendarEventCommand, ServiceResult<CalendarEventResponseDto>>
    {
        private readonly IRepository<CalendarEvent> _eventRepo;
        private readonly IRepository<CalendarModel> _calendarRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateCalendarEventCommandHandler(
            IRepository<CalendarEvent> eventRepo,
            IRepository<CalendarModel> calendarRepo,
            IHttpContextAccessor httpContextAccessor)
        {
            _eventRepo = eventRepo;
            _calendarRepo = calendarRepo;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResult<CalendarEventResponseDto>> Handle(
            CreateCalendarEventCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.GetUserId<long>();

            var date = request.Input.Date.Date;

            var calendarRow = await _calendarRepo.TableNoTracking
                .Where(c => c.Date == date)
                .Select(c => new { c.Id })
                .FirstOrDefaultAsync(cancellationToken);

            if (calendarRow is null)
                return ServiceResult.BadRequest<CalendarEventResponseDto>(
                    "Calendar day not found. Make sure calendar is seeded for the given date.");

            var ev = CalendarEvent.Create(
                calendarId: calendarRow.Id,
                userId: userId,
                date: date,
                title: request.Input.Title,
                description: request.Input.Description,
                type: request.Input.Type
            );

            await _eventRepo.AddAsync(ev, cancellationToken);

            return ServiceResult.Ok(new CalendarEventResponseDto(
                Id: ev.Id,
                Date: ev.Date,
                Title: ev.Title,
                Description: ev.Description,
                Type: ev.Type,
                CreatedAt: ev.CreatedAt
            ));
        }
    }

}

