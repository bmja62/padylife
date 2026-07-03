using Application.Baskets.DTOs;
using Application.Cqrs.Commands;
using Common.Utilities;
using Data.Contracts;
using Entities.Baskets;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Baskets.Commands
{
    public class RemoveItemFromBasketCommand : ICommand<ServiceResult>
    {
        public RemoveItemFromBasketCommand(RemoveItemFromBasketCommandDTO input)
        {
            Input = input;
        }

        public RemoveItemFromBasketCommandDTO Input { get; }
    }

    public class RemoveItemFromBasketCommandHandler(
        IRepository<Basket> basketRepository,
        IRepository<BasketItem> basketItemRepository,
        IHttpContextAccessor httpContextAccessor
    ) : ICommandHandler<RemoveItemFromBasketCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(RemoveItemFromBasketCommand request, CancellationToken cancellationToken)
        {
            var userId = httpContextAccessor.HttpContext.User.Identity.GetUserId<long>();
            if (userId <= 0)
                return ServiceResult.Fail("کاربر معتبر نیست");

            var basket = await basketRepository.Table.Include(t => t.Items).FirstOrDefaultAsync(t => t.UserId == userId && t.Status == BasketStatus.Active, cancellationToken);
            if (basket == null)
                return ServiceResult.Fail("سبد خرید فعالی برای کاربر یافت نشد");

            var basketItem = await basketItemRepository.Table.Where(t => t.ObjectId == request.Input.ItemId && t.ItemType == request.Input.BasketItemType && t.BasketId == basket.Id).FirstOrDefaultAsync(cancellationToken);
            if (basketItem == null || basketItem.BasketId != basket.Id)
                return ServiceResult.Fail("آیتم مورد نظر در سبد خرید پیدا نشد");

            await basketItemRepository.DeleteAsync(basketItem, cancellationToken);

            basket.AddHistory("حذف آیتم", $"آیتم با شناسه {basketItem.ObjectId} از سبد حذف شد.");
            basket.LastUpdated = DateTime.UtcNow;

            await basketRepository.UpdateAsync(basket, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
