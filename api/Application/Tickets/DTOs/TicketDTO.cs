
using Entities.Tickets;

namespace Application.Tickets.DTOs
{
    public class TicketDTO
    {
        public long Id { get; set; }
        public long UserId { get; internal set; }
        public TicketStatus Status { get; internal set; }
        public string Title { get; internal set; }
        public DateTime CreatedAt { get; internal set; }
        public List<TicketDetailDTO> TicketDetails { get; set; }
        public DateTime? UpdatedAt { get; internal set; }
        public TicketType TicketType { get; internal set; }
    }

    public class TicketCountDTO
    {
        public long Count { get; set; }
    }
}
