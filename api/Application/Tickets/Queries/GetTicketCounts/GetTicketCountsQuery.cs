using Application.Cqrs.Queris;
using Application.Tickets.DTOs;
using Data.Contracts;
using Entities.Tickets;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Tickets.Queries.GetTicketCounts
{
    public class GetTicketCountsQuery(long? userId, TicketStatus? status) : IQuery<ServiceResult<TicketCountDTO>>
    {
        public long? UserId { get; } = userId;
        public TicketStatus? Status { get; set; } = status;
    }

    public class GetTicketCountsQueryHandler
        (IRepository<Ticket> repository) : IQueryHandler<GetTicketCountsQuery, ServiceResult<TicketCountDTO>>
    {

        public async Task<ServiceResult<TicketCountDTO>> Handle(GetTicketCountsQuery request, CancellationToken cancellationToken)
        {
            var query = repository.Table;
            if (request.UserId.HasValue)
                query = query.Where(w => w.UserId == request.UserId);

            if (request.Status.HasValue)
                query = query.Where(w => w.Status == request.Status);

            var count = await query.CountAsync(cancellationToken);

            return ServiceResult.Ok(new TicketCountDTO
            {
                Count = count
            });
        }
    }
}
