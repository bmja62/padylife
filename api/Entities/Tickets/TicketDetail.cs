

using Entities.Common;
using Entities.Users;

namespace Entities.Tickets
{
    public class TicketDetail : BaseEntity<long>
    {
        internal TicketDetail() { }



        public Ticket Ticket { get; set; }
        public long TicketId { get; set; }
        public string Content { get; internal set; }
        public long? SupportUserId { get; internal set; }
        public User SupportUser { get; set; }
        public TicketDetailResponseType ResponseType { get; internal set; }
    }


}
