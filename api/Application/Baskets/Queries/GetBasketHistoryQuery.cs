using Application.Baskets.DTOs;
using Application.Cqrs.Queris;
using Data.Contracts;
using Entities.Baskets;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Baskets.Queries
{
    public class GetBasketHistoryQuery : IQuery<ServiceResult<List<BasketHistoryDTO>>>
    {
        public GetBasketHistoryQuery(long basketId)
        {
            BasketId = basketId;
        }

        public long BasketId { get; }
    }

    public class GetBasketHistoryQueryHandler(
        IRepository<BasketHistory> basketHistoryRepository
    ) : IQueryHandler<GetBasketHistoryQuery, ServiceResult<List<BasketHistoryDTO>>>
    {
        public async Task<ServiceResult<List<BasketHistoryDTO>>> Handle(GetBasketHistoryQuery request, CancellationToken cancellationToken)
        {

            var history = await basketHistoryRepository
                .TableNoTracking
                .Where(t => t.BasketId == request.BasketId)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new BasketHistoryDTO
                {
                    Title = t.Title,
                    Description = t.Description,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return ServiceResult.Ok(history);
        }
    }
}
