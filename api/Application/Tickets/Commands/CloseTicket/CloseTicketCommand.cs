using Application.Cqrs.Commands;
using Services;

namespace Application.Tickets.Commands.CloseTicket
{
    public class CloseTicketCommand(long ticketId) : ICommand<ServiceResult>
    {
        public long TicketId { get; } = ticketId;
    }
}
