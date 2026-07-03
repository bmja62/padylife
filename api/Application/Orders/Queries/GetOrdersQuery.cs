using Application.Cqrs.Queris;
using Application.Orders.DTOs;
using Common.GridResults;
using Data.Contracts;
using Entities.Orders;
using Entities.Plans;
using Entities.Products;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Orders.Queries
{
    public class GetOrdersQuery : IQuery<ServiceResult<GlobalGridResult<OrderListItemDTO>>>
    {
        public long? UserId { get; set; }
        public string? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public long? OrderId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetOrdersQueryHandler : IQueryHandler<GetOrdersQuery, ServiceResult<GlobalGridResult<OrderListItemDTO>>>
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductVariant> _variantRepository;
        private readonly IRepository<Plan> _planRepository;
        private readonly IRepository<ExpertPlanPrice> _expertPlanPriceRepository;


        public GetOrdersQueryHandler(
            IRepository<Order> orderRepository,
            IRepository<Product> productRepository,
            IRepository<ProductVariant> variantRepository,
            IRepository<Plan> planRepository,
            IRepository<ExpertPlanPrice> expertPlanPriceRepository
            )
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _variantRepository = variantRepository;
            _planRepository = planRepository;
            _expertPlanPriceRepository = expertPlanPriceRepository;
        }

        public async Task<ServiceResult<GlobalGridResult<OrderListItemDTO>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            var userId = request.UserId;

            var query = _orderRepository.Table
                .Include(o => o.Items).AsQueryable();

            if (userId.HasValue && userId.Value > 0)
                query = query.Where(t => t.UserId == userId.Value);

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (Enum.TryParse<OrderStatus>(request.Status, true, out var status))
                    query = query.Where(o => o.Status == status);
            }

            if (request.FromDate.HasValue)
                query = query.Where(o => o.CreatedAt >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(o => o.CreatedAt <= request.ToDate.Value);

            if (request.OrderId.HasValue)
                query = query.Where(o => o.Id == request.OrderId.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var orders = await query
                .Include(t => t.Address).ThenInclude(t => t.Country)
                .Include(t => t.Address).ThenInclude(t => t.Province)
                .Include(t => t.Address).ThenInclude(t => t.City)
                .Include(t => t.User)
                .Include(t => t.Items)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var data = await MapOrdersToDtoAsync(orders, cancellationToken);

            return ServiceResult.Ok(new GlobalGridResult<OrderListItemDTO>
            {
                Data = data,
                TotalCount = totalCount,
            });
        }
        private async Task<string?> GetProductTitleAsync(long productId)
        {
            return await _productRepository.Table
                .Where(p => p.Id == productId)
                .Select(p => p.Name)
                .FirstOrDefaultAsync();
        }
        private async Task<string> GetPlanTitleAsync(long planId)
        {
            return await _planRepository.Table
              .Where(p => p.Id == planId)
              .Select(p => p.Title)
              .FirstOrDefaultAsync();
        }
        private async Task<string> GetExpertPlanTitleAsync(long expertPlanPriceId)
        {
            return await _expertPlanPriceRepository.Table
                .Include(t => t.Expert)
                .Include(t => t.Plan)
          .Where(p => p.Id == expertPlanPriceId)
          .Select(p => "متخصص : " + p.Expert.FullName + " - " + " پلن : " + p.Plan.Title)
          .FirstOrDefaultAsync();
        }

        private async Task<string?> GetVariantTitleAsync(long variantId)
        {
            var variant = await _variantRepository.Table
                .Where(v => v.Id == variantId)
                .Select(v => new
                {
                    v.SKU,
                    ProductName = _productRepository.Table
                    .Where(p => p.Variants.Any(vv => vv.Id == variantId))
                    .Select(p => p.Name)
                    .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            if (variant == null || variant.ProductName == null)
                return null;

            return $"{variant.ProductName} - {variant.SKU}";
        }

        private async Task<List<OrderListItemDTO>> MapOrdersToDtoAsync(List<Order> orders, CancellationToken cancellationToken)
        {
            var result = new List<OrderListItemDTO>();

            foreach (var order in orders)
            {
                var dto = new OrderListItemDTO
                {
                    OrderId = order.Id,
                    OrderDate = order.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                    Status = order.Status.ToString(),
                    TotalPrice = order.TotalAmount,
                    UserId = order.UserId,
                    Address = order.Address?.GetFullAddress() ?? "-",
                    UserInfo = order.User == null ? null : new UserOrderInfoDTO
                    {
                        Id = order.User.Id,
                        FullName = order.User.FullName,
                        UserName = order.User.UserName,
                        Email = order.User.Email,
                        PhoneNumber = order.User.PhoneNumber
                    },
                    Items = new List<OrderItemSummaryDTO>()
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

                    dto.Items.Add(new OrderItemSummaryDTO
                    {
                        ObjectId = item.ObjectId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        ItemType = item.ItemType.ToString(),
                        Title = title ?? "-"
                    });
                }

                result.Add(dto);
            }

            return result;
        }


    }

}
