using Application.Baskets.DTOs;
using Application.Baskets.Events;
using Application.Cqrs.Events;
using Application.Cqrs.Queris;
using Data.Contracts;
using Entities.Baskets;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Baskets.Queries
{
    public class GetOrCreateBasketQuery(long userId) : IQuery<ServiceResult<BasketDTO>>
    {
        public long UserId { get; } = userId;
    }

    public class GetOrCreateBasketQueryHandler(IRepository<Basket> basketRepository, IDomainEventDispatcher domainEventDispatcher) : IQueryHandler<GetOrCreateBasketQuery, ServiceResult<BasketDTO>>
    {
        private readonly IRepository<Basket> _basketRepository = basketRepository;

        public async Task<ServiceResult<BasketDTO>> Handle(GetOrCreateBasketQuery request, CancellationToken cancellationToken)
        {
            var basket = await _basketRepository.Table
                .Where(b => b.UserId == request.UserId)
                .Include(b => b.Items)
                .FirstOrDefaultAsync(cancellationToken);

            if (basket == null)
            {
                basket = Basket.CreateByUserId(request.UserId);
                await _basketRepository.AddAsync(basket, cancellationToken);
            }

            //فراخوانی حذف سبد های غیر فعال
            basket.AddDomainEvent(new ExpireBasketsEvent());

            var basketDto = new BasketDTO
            {
                Id = basket.Id,
                UserId = basket.UserId,
                CreatedAt = basket.CreatedAt,
                DiscountAmount = basket.DiscountAmount,
                FinalPrice = basket.FinalPrice,
                LastUpdated = basket.LastUpdated,
                ProductTotalPrice = basket.ProductTotalPrice,
                ShippingCost = basket.ShippingCost,
                Status = basket.Status,
                Items = basket.Items.Select(i => new BasketItemDTO
                {
                    Id = i.Id,
                    ObjectId = i.ObjectId,
                    ItemType = i.ItemType,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            return ServiceResult.Ok(basketDto);
        }
    }
}
