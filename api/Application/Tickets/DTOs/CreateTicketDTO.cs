using Entities.Tickets;

namespace Application.Tickets.DTOs
{
    public class CreateTicketDTO
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public TicketType TicketType { get; set; }
    }
}
