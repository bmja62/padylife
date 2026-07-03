using Entities.Tickets;

namespace Application.Tickets.DTOs
{
    public class TicketDetailDTO
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public long? SupportUserId { get; set; }
        public TicketDetailResponseType ResponseType { get; set; }
        public DateTime CreatedAt { get; internal set; }
    }
}
