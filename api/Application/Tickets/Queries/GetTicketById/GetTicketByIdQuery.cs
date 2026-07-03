using Application.Cqrs.Queris;
using Application.Tickets.DTOs;
using Services;

namespace Application.Tickets.Queries.GetTicketById
{
    public class GetTicketByIdQuery(long ticketId) : IQuery<ServiceResult<TicketDTO>>
    {
        public long TicketId { get; } = ticketId;
    }
}
