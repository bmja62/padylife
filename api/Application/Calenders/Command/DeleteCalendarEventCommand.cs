using Application.Cqrs.Commands;
using Common.Utilities;
using Data.Contracts;
using Entities.Calendar;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Calenders.Command
{
    public record DeleteCalendarEventCommand(long Id)
      : ICommand<ServiceResult>;

    public class DeleteCalendarEventCommandHandler
        : ICommandHandler<DeleteCalendarEventCommand, ServiceResult>
    {
        private readonly IRepository<CalendarEvent> _eventRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteCalendarEventCommandHandler(IRepository<CalendarEvent> eventRepo, IHttpContextAccessor httpContextAccessor)
        {
            _eventRepo = eventRepo;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResult> Handle(DeleteCalendarEventCommand request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.User.Identity.GetUserId<long>();

            var ev = await _eventRepo.Table.FirstOrDefaultAsync(e => e.Id == request.Id && e.UserId == userId, cancellationToken);
            if (ev is null)
                return ServiceResult.NotFound("Event not found.");

            await _eventRepo.SoftDeleteAsync(ev, cancellationToken);

            return ServiceResult.Ok();
        }
    }

}
