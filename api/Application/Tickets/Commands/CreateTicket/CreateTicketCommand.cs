using Application.Cqrs.Commands;
using Entities.Tickets;
using Services;

namespace Application.Tickets.Commands.CreateTicket
{
    public class CreateTicketCommand(string title, string content, TicketType ticketType) : ICommand<ServiceResult>
    {
        public string Title { get; set; } = title;
        public string Content { get; } = content;
        public TicketType TicketType { get; } = ticketType;
    }
}
