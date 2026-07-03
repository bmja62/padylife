using Application.Cqrs.Queris;
using Application.Tickets.DTOs;
using Common.GridResults;
using Services;

namespace Application.Tickets.Queries.GetUserTickets
{
    public class GetUserTicketsQuery : GlobalGrid, IQuery<ServiceResult<GlobalGridResult<TicketDTO>>>
    {
        public GetUserTicketsQuery(int page, int count, long userId)
        {
            PageNumber = page;
            Count = count;
            UserId = userId;
        }

        public long UserId { get; }
    }
}
