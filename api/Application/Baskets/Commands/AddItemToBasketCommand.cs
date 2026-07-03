using Application.Baskets.DTOs;
using Application.Baskets.Events;
using Application.Cqrs.Commands;
using Application.Cqrs.Events;
using Data.Contracts;
using Entities.Baskets;
using Entities.Plans;
using Entities.Products;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Services.StockManagerServices;

namespace Application.Baskets.Commands
{
    public class AddItemToBasketCommand(long userId, AddBasketItemDTO input) : ICommand<ServiceResult>
    {
        public long UserId { get; } = userId;
        public AddBasketItemDTO Input { get; } = input;
    }
    public class AddItemToBasketCommandHandler(
        IStockManagerService stockManagerService,
        IRepository<Basket> basketRepository,
        IRepository<Product> productRepository,
        IRepository<ProductVariant> productVariantRepository,
        IRepository<ExpertPlanPrice> expertPlanPriceRepository,
        IRepository<Plan> planRepository,
        IDomainEventDispatcher domainEventDispatcher
        ) : ICommandHandler<AddItemToBasketCommand, ServiceResult>
    {
        private readonly IRepository<Basket> _basketRepository = basketRepository;
        private readonly IRepository<Product> _productRepository = productRepository;
        private readonly IRepository<ProductVariant> _productVariantRepository = productVariantRepository;
        private readonly IRepository<ExpertPlanPrice> _expertPlanPriceRepository = expertPlanPriceRepository;
        private readonly IRepository<Plan> _planRepository = planRepository;

        public async Task<ServiceResult> Handle(AddItemToBasketCommand request, CancellationToken cancellationToken)
        {
            var basket = await _basketRepository.Table.Include(t => t.Items)
                .FirstOrDefaultAsync(b => b.UserId == request.UserId, cancellationToken);

            if (basket == null)
            {
                basket = Basket.CreateByUserId(request.UserId);
                await _basketRepository.AddAsync(basket, cancellationToken);
            }
            basket.AddDomainEvent(new ExpireBasketsEvent());

            var unitPrice = request.Input.ItemType == BasketItemType.Product ?
                            await _productRepository.Table.Where(t => t.Id == request.Input.ObjectId).Select(t => t.Price).FirstOrDefaultAsync() :
                            request.Input.ItemType == BasketItemType.Variant ?
                            await _productRepository.Table.SelectMany(t => t.Variants).Where(t => t.Id == request.Input.ObjectId).Select(t => t.Price).FirstOrDefaultAsync() :
                            request.Input.ItemType == BasketItemType.ExpertPlanPrice ?
                            await _expertPlanPriceRepository.Table.Where(t => t.Id == request.Input.ObjectId).Select(t => t.Price).FirstOrDefaultAsync() :
                            request.Input.ItemType == BasketItemType.Plan ?
                            await _planRepository.Table.Where(t => t.Id == request.Input.ObjectId && t.Price != null && t.Price.HasValue && t.Price.Value > 0).Select(t => t.FinalPrice.Value).FirstOrDefaultAsync()
                            : 0
                            ;
            if (unitPrice < 0)
                return ServiceResult.BadRequest("قیمت محصول نامعتبر میباشد");


            if (basket.Items.Any(t => t.ObjectId == request.Input.ObjectId && t.ItemType == request.Input.ItemType))
                return ServiceResult.BadRequest("این محصول در سبد خرید شما وجود دارد");

            //بررسی موجودی آیتم
            int availableStock = 0;
            if (request.Input.ItemType == BasketItemType.Product)
            {
                var product = await _productRepository.Table.Include(t => t.Variants).Where(t => t.Id == request.Input.ObjectId).FirstOrDefaultAsync();
                if (product.Variants != null && product.Variants.Any() && product.Variants.Count > 0)
                    return ServiceResult.BadRequest("این آیتم دارای متغییر می باشد لطفا یکی از متغییر های آن را به سبد خرید اضافه نمایید");
                availableStock = await stockManagerService.GetAvailableStockAsync(product, null);
                if (availableStock <= 0)
                    return ServiceResult.BadRequest("محصول ناموجود شده");
                if (!await stockManagerService.HasStockAsync(product, request.Input.Quantity))
                {
                    if (request.Input.Quantity > availableStock)
                        request.Input.Quantity = availableStock;
                }
            }
            else if (request.Input.ItemType == BasketItemType.Variant)
            {
                var productVariant = await _productVariantRepository.Table.Where(t => t.Id == request.Input.ObjectId).FirstOrDefaultAsync();
                availableStock = await stockManagerService.GetAvailableStockAsync(productVariant, null);
                if (availableStock <= 0)
                    return ServiceResult.BadRequest("محصول ناموجود شده");
                if (!await stockManagerService.HasStockAsync(productVariant, request.Input.Quantity))
                {
                    if (request.Input.Quantity > availableStock)
                        request.Input.Quantity = availableStock;
                }
            }
            if (availableStock <= 0 && (request.Input.ItemType == BasketItemType.Product || request.Input.ItemType == BasketItemType.Variant))
                return ServiceResult.BadRequest("موجودی انبار کافی نیست");

            var newItem = new BasketItem
            {
                BasketId = basket.Id,
                ObjectId = request.Input.ObjectId,
                ItemType = request.Input.ItemType,
                Quantity = request.Input.Quantity,
                UnitPrice = unitPrice
            };

            basket.AddItem(newItem);
            await _basketRepository.UpdateAsync(basket, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
