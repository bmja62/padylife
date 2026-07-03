using Application.Cqrs.Queris;
using Application.Orders.DTOs;
using Common.Utilities;
using Data.Contracts;
using Entities.Orders;
using Entities.Plans;
using Entities.Products;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Orders.Queries
{
    public class GetOrderDetailsQuery(long orderId) : IQuery<ServiceResult<OrderDetailsDTO>>
    {
        public long OrderId { get; set; } = orderId;
    }

    public class GetOrderDetailsQueryHandler(
    IRepository<Order> orderRepository,
    IRepository<Product> productRepository,
    IRepository<ProductVariant> variantRepository,
    IRepository<Plan> planRepository,
    IRepository<ExpertPlanPrice> expertPlanPriceRepository
    ) : IQueryHandler<GetOrderDetailsQuery, ServiceResult<OrderDetailsDTO>>
    {
        public async Task<ServiceResult<OrderDetailsDTO>> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
        {
            // واکشی سفارش با تمام نیازمندی‌ها
            var order = await orderRepository.Table
                .Include(t => t.Address).ThenInclude(t => t.Country)
                .Include(t => t.Address).ThenInclude(t => t.Province)
                .Include(t => t.Address).ThenInclude(t => t.City)
                .Include(o => o.Items)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

            if (order == null)
                return ServiceResult.NotFound<OrderDetailsDTO>("سفارش یافت نشد");

            var orderDto = new OrderDetailsDTO
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                Status = order.Status.ToDisplay(DisplayProperty.Name),
                PaymentStatus = order.PaymentStatus.ToDisplay(DisplayProperty.Name),
                TotalAmount = order.TotalAmount,
                UserId = order.UserId,
                Address = order.Address?.GetFullAddress() ?? "-",
                UserInfo = new UserOrderInfoDTO
                {
                    Id = order.User.Id,
                    FullName = order.User.FullName,
                    UserName = order.User.UserName,
                    Email = order.User.Email ?? "-",
                    PhoneNumber = string.Join('-', new string[]
                    {
                    order.User.PhoneNumber,
                    order.Address?.LandlinePhone ?? "تلفن ثابت ندارد",
                    order.Address?.RecipientPhone ?? "تلفن گیرنده ندارد",
                    })
                },
                Items = new List<OrderItemDTO>()
            };

            foreach (var item in order.Items)
            {
                string? title = item.ItemType switch
                {
                    OrderItemType.Product => await GetProductTitleAsync(item.ObjectId),
                    OrderItemType.Variant => await GetVariantTitleAsync(item.ObjectId),
                    OrderItemType.Plan => await GetPlanTitleAsync(item.ObjectId),
                    OrderItemType.ExpertPlanPrice => await GetExpertPlanTitleAsync(item.ObjectId),
                    _ => "-"
                };

                orderDto.Items.Add(new OrderItemDTO
                {
                    ObjectId = item.ObjectId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    ItemType = item.ItemType.ToString(),
                    Title = title ?? "-"
                });
            }

            return ServiceResult.Ok(orderDto);
        }
        private async Task<string> GetPlanTitleAsync(long planId)
        {
            return await planRepository.Table
              .Where(p => p.Id == planId)
              .Select(p => p.Title)
              .FirstOrDefaultAsync();
        }
        private async Task<string> GetExpertPlanTitleAsync(long expertPlanPriceId)
        {
            return await expertPlanPriceRepository.Table
                .Include(t => t.Expert)
                .Include(t => t.Plan)
          .Where(p => p.Id == expertPlanPriceId)
          .Select(p => "متخصص : " + p.Expert.FullName + " - " + " پلن : " + p.Plan.Title)
          .FirstOrDefaultAsync();
        }
        private async Task<string?> GetProductTitleAsync(long productId)
        {
            return await productRepository.Table
                .Where(p => p.Id == productId)
                .Select(p => p.Name)
                .FirstOrDefaultAsync();
        }

        private async Task<string?> GetVariantTitleAsync(long variantId)
        {
            var variant = await variantRepository.Table
                .Where(v => v.Id == variantId)
                .Select(v => new
                {
                    v.SKU,
                    ProductName = productRepository.Table
                        .Where(p => p.Variants.Any(vv => vv.Id == variantId))
                        .Select(p => p.Name)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (variant == null || variant.ProductName == null)
                return null;

            return $"{variant.ProductName} - {variant.SKU}";
        }
    }

}

