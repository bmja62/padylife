using Application.Cqrs.Commands;
using Common.Roles;
using Common.Utilities;
using Data.Contracts;
using Entities.Tickets;
using Microsoft.AspNetCore.Http;
using Services;

namespace Application.Tickets.Commands.AnswerToTicket
{
    public class AnswerToTicketCommandHandler(
        IRepository<Ticket> repository,
        IHttpContextAccessor httpContextAccessor
        ) : ICommandHandler<AnswerToTicketCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(AnswerToTicketCommand command, CancellationToken cancellationToken = default)
        {
            var isAdmin = httpContextAccessor.HttpContext.User.IsInRole(UserRoles.Admin.Name);
            var userId = httpContextAccessor.HttpContext.User.Identity.GetUserId<long>();

            var ticket = await repository.GetByIdAsync(cancellationToken, command.TicketId);
            if (ticket is null)
                ServiceResult.BadRequest("تیکت یافت نشد");

            if (ticket.UserId != userId && !isAdmin)
                return ServiceResult.BadRequest("شما دسترسی به این منبع ندارید");

            if (isAdmin)
            {
                ticket.AddSupportResponse(command.Content, userId);
            }
            else
            {
                ticket.AddUserResponse(command.Content);
            }

            repository.Update(ticket);
            return ServiceResult.Ok();
        }
    }
}
