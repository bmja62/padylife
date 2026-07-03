using Application.Cqrs.Queris;
using Application.Tickets.DTOs;
using Common.GridResults;
using Data.Contracts;
using Entities.Tickets;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Tickets.Queries.GetTickets
{
    public class GetTicketsQueryHandler(IRepository<Ticket> ticketRepository) : IQueryHandler<GetTicketsQuery, ServiceResult<GlobalGridResult<TicketDTO>>>
    {

        public async Task<ServiceResult<GlobalGridResult<TicketDTO>>> Handle(GetTicketsQuery request, CancellationToken cancellationToken)
        {
            request.DefaultPagination();
            var query = ticketRepository.Table
                .OrderByDescending(x => x.UpdatedAt.HasValue ? x.UpdatedAt : x.CreatedAt)
                .AsQueryable();


            if (request.TicketType.HasValue)
                query = query.Where(x => x.TicketType == request.TicketType);


            if (request.TicketStatus.HasValue)
                query = query.Where(x => x.Status == request.TicketStatus);


            var data =
                await
                query.Select(s => new TicketDTO
                {
                    TicketType = s.TicketType,
                    Id = s.Id,
                    UpdatedAt = s.UpdatedAt,
                    UserId = s.UserId,
                    Status = s.Status,
                    Title = s.Title,
                    CreatedAt = s.CreatedAt,
                }).Skip(request.Skip)
                .Take(request.Take)
                .ToListAsync(cancellationToken);


            var count = await query.CountAsync(cancellationToken);

            var page = new GlobalGridResult<TicketDTO>()
            {
                TotalCount = count,
                Data = data,
            };

            return ServiceResult.Ok(page);
        }
    }
}
