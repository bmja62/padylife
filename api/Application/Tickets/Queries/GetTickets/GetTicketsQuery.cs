using Application.Cqrs.Queris;
using Application.Tickets.DTOs;
using Common.GridResults;
using Entities.Tickets;
using Services;

namespace Application.Tickets.Queries.GetTickets
{
    public class GetTicketsQuery : GlobalGrid, IQuery<ServiceResult<GlobalGridResult<TicketDTO>>>
    {
        public GetTicketsQuery(int page, int count, TicketType? ticketType, TicketStatus? ticketStatus)
        {
            PageNumber = page;
            Count = count;
            TicketType = ticketType;
            TicketStatus = ticketStatus;
        }


        public TicketType? TicketType { get; }
        public TicketStatus? TicketStatus { get; }
    }
}
