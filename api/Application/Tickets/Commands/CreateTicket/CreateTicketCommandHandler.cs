using Application.Cqrs.Commands;
using Common.Roles;
using Common.Utilities;
using Data.Contracts;
using Entities.Tickets;
using Microsoft.AspNetCore.Http;
using Services;

namespace Application.Tickets.Commands.CreateTicket
{
    public class CreateTicketCommandHandler
        (
        IRepository<Ticket> repository,
        IHttpContextAccessor httpContextAccessor
        ) : ICommandHandler<CreateTicketCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(CreateTicketCommand command, CancellationToken cancellationToken)
        {
            var userId = httpContextAccessor.HttpContext.User.Identity.GetUserId<long>();
            var isUser = httpContextAccessor.HttpContext.User.IsInRole(UserRoles.User.Name);
            var tickect = Ticket.Create(command.Title, userId, TicketStatus.WaitingForSupport, command.TicketType, SupportType.Support);
            tickect.AddUserResponse(command.Content);
            await repository.AddAsync(tickect, cancellationToken);
            return ServiceResult.Ok();
        }
    }
}
