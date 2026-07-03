using Application.Cqrs.Commands;
using Common.Roles;
using Common.Utilities;
using Data.Contracts;
using Entities.Tickets;
using Microsoft.AspNetCore.Http;
using Services;

namespace Application.Tickets.Commands.CloseTicket
{
    public class CloseTicketCommandHandler
        (
        IRepository<Ticket> repository,
        IHttpContextAccessor httpContextAccessor) :
        ICommandHandler<CloseTicketCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(CloseTicketCommand command, CancellationToken cancellationToken = default)
        {
            var isAdmin = httpContextAccessor.HttpContext.User.IsInRole(UserRoles.Admin.Name);
            var userId = httpContextAccessor.HttpContext.User.Identity.GetUserId<long>();

            var ticket = await repository.GetByIdAsync(cancellationToken, command.TicketId);
            if (ticket is null)
                ServiceResult.BadRequest("تیکت یافت نشد");

            if (ticket.UserId != userId && !isAdmin)
                return ServiceResult.BadRequest("شما دسترسی به این منبع ندارید");

            ticket.Close();
            repository.Update(ticket);
            return ServiceResult.Ok();
        }
    }
}
