using Application.Cqrs.Queris;
using Application.Tickets.DTOs;
using Common.GridResults;
using Data.Contracts;
using Entities.Tickets;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Tickets.Queries.GetUserTickets
{
    public class GetUserTicketsQueryHandler(IRepository<Ticket> repository) :
        IQueryHandler<GetUserTicketsQuery, ServiceResult<GlobalGridResult<TicketDTO>>>
    {
        public async Task<ServiceResult<GlobalGridResult<TicketDTO>>> Handle(GetUserTicketsQuery request, CancellationToken cancellationToken)
        {
            request.DefaultPagination();
            var query = repository.Table.Where(x => x.UserId == request.UserId);

            var data =
                await
                query
                .OrderByDescending(x => x.UpdatedAt.HasValue ? x.UpdatedAt : x.CreatedAt)
                .Select(s => new TicketDTO
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    Status = s.Status,
                    Title = s.Title,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    TicketType = s.TicketType,
                })
                .Skip((request.Skip))
                .Take(request.Take)
                .ToListAsync(cancellationToken);

            var count = await query.CountAsync(cancellationToken);

            var pageModel = new GlobalGridResult<TicketDTO>
            {
                TotalCount = count,
                Data = data
            };
            return ServiceResult.Ok(pageModel);
        }
    }
}
