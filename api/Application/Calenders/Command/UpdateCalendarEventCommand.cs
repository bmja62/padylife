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
    public record UpdateCalendarEventCommand(UpdateCalendarEventDto Input)
      : ICommand<ServiceResult<CalendarEventResponseDto>>;

    public class UpdateCalendarEventCommandHandler
        : ICommandHandler<UpdateCalendarEventCommand, ServiceResult<CalendarEventResponseDto>>
    {
        private readonly IRepository<CalendarEvent> _eventRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateCalendarEventCommandHandler(
            IRepository<CalendarEvent> eventRepo,
            IHttpContextAccessor httpContextAccessor)
        {
            _eventRepo = eventRepo;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResult<CalendarEventResponseDto>> Handle(UpdateCalendarEventCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.GetUserId<long>();

            var ev = await _eventRepo.Table.FirstOrDefaultAsync(e => e.Id == request.Input.Id && e.UserId == userId, cancellationToken);
            if (ev is null)
                   return ServiceResult.NotFound<CalendarEventResponseDto>("Event not found.");

            ev.Update(request.Input.Title, request.Input.Description, request.Input.Type);

            await _eventRepo.UpdateAsync(ev, cancellationToken);

            return ServiceResult.Ok(new CalendarEventResponseDto(
                ev.Id, ev.Date, ev.Title, ev.Description, ev.Type, ev.CreatedAt));
        }
    }
}
