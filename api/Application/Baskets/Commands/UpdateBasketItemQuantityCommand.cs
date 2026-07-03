using Application.Baskets.DTOs;
using Application.Baskets.Events;
using Application.Cqrs.Commands;
using Application.Cqrs.Events;
using Common.Utilities;
using Data.Contracts;
using Entities.Baskets;
using Entities.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Services.StockManagerServices;

namespace Application.Baskets.Commands
{

    public class UpdateBasketItemQuantityCommand(UpdateBasketItemQuantityCommandDTO input) : ICommand<ServiceResult>
    {
        public UpdateBasketItemQuantityCommandDTO Input { get; } = input;
    }

    public class UpdateBasketItemQuantityCommandHandler(
        IDomainEventDispatcher domainEventDispatcher,
        IStockManagerService stockManagerService,
        IRepository<Product> productRepository,
        IRepository<ProductVariant> productVariantRepository,
         IRepository<Basket> basketRepository,
         IRepository<BasketItem> basketItemRepository,
         IHttpContextAccessor httpContextAccessor
     ) : ICommandHandler<UpdateBasketItemQuantityCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(UpdateBasketItemQuantityCommand request, CancellationToken cancellationToken)
        {
            var userId = httpContextAccessor.HttpContext.User.Identity.GetUserId<long>();
            if (userId <= 0)
                return ServiceResult.Fail("کاربر معتبر نیست");

            var basket = await basketRepository.Table.FirstOrDefaultAsync(t => t.UserId == userId && t.Status == BasketStatus.Active, cancellationToken);
            if (basket == null)
                return ServiceResult.Fail("سبد خرید فعال پیدا نشد");

            //فراخوانی حذف سبد های غیر فعال
            basket.AddDomainEvent(new ExpireBasketsEvent());


            var basketItem = await basketItemRepository.Table.Where(t => t.ObjectId == request.Input.ItemId && t.ItemType == request.Input.ItemType && t.BasketId == basket.Id).FirstOrDefaultAsync(cancellationToken);
            if (basketItem == null || basketItem.BasketId != basket.Id)
                return ServiceResult.Fail("آیتم مورد نظر در سبد خرید پیدا نشد");

            if (request.Input.NewQuantity <= 0)
            {
                // اگر تعداد جدید صفر یا منفی باشد، آیتم حذف شود
                await basketItemRepository.DeleteAsync(basketItem, cancellationToken);
                basket.AddHistory("حذف آیتم", $"آیتم با شناسه {basketItem.ObjectId} از سبد حذف شد.");
            }
            else
            {
                int stockQuantity = 0;
                if (basketItem.ItemType == BasketItemType.Product)
                {
                    var productInDb = await productRepository.GetByIdAsync(cancellationToken, basketItem.ObjectId);
                    stockQuantity = await stockManagerService.GetAvailableStockAsync(productInDb, null);
                }
                else if (basketItem.ItemType == BasketItemType.Variant)
                {
                    var productVariantInDb = await productVariantRepository.GetByIdAsync(cancellationToken, basketItem.ObjectId);
                    stockQuantity = await stockManagerService.GetAvailableStockAsync(productVariantInDb, null);
                }
                if (stockQuantity > request.Input.NewQuantity)
                {
                    basketItem.Quantity = request.Input.NewQuantity;
                    await basketItemRepository.UpdateAsync(basketItem, cancellationToken);
                    basket.AddHistory("بروزرسانی تعداد", $"تعداد آیتم با شناسه {basketItem.ObjectId} به {basketItem.Quantity} تغییر یافت.");
                }
                else
                {
                    return ServiceResult.Fail("موجودی محصول کافی نمیباشد");
                }
            }

            basket.LastUpdated = DateTime.UtcNow;
            await basketRepository.UpdateAsync(basket, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
