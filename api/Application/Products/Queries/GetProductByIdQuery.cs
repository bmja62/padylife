using Application.Baskets.Queries;
using Application.Cqrs.Queris;
using Application.Products.DTOs;
using Common.Utilities;
using Data.Contracts;
using Entities.Baskets;
using Entities.Medias;
using Entities.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Services.MediaServices;

namespace Application.Products.Queries
{
    public class GetProductByIdQuery : IQuery<ServiceResult<GetProductByIdDTO>>
    {
        public GetProductByIdQuery(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }

    public class GetProductByIdQueryHandler(
        IRepository<Product> productRepository,
        IMediaService mediaService,
        IQueryDispatcher queryDispatcher,
        IHttpContextAccessor accessor
        ) : IQueryHandler<GetProductByIdQuery, ServiceResult<GetProductByIdDTO>>
    {
        public async Task<ServiceResult<GetProductByIdDTO>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await productRepository.TableNoTracking
                .Include(p => p.Category)
                .Include(p => p.AttributeValues)
                    .ThenInclude(av => av.Attribute)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.AttributeValues)
                    .ThenInclude(vav => vav.Attribute)
                .Include(p => p.CreatedByUser)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (product == null)
                return ServiceResult.BadRequest<GetProductByIdDTO>("محصول یافت نشد");

            // پردازش variantها و استخراج گزینه‌های موجود
            var variants = ProcessVariants(product.Variants);
            var availableOptions = ExtractAvailableOptions(variants);
            var basketQuantities = await ProcessUserBasketQuantity(product, variants);

            var result = new GetProductByIdDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                Type = product.Type,
                BasketQuantity = basketQuantities.ProductQuantity,
                ProductImages = GetProductImageDTO.Create(
                                                    mediaService.GetObjectMedia(product.Id, MediaTypes.ProductMain),
                                                    mediaService.GetObjectMedias(product.Id, MediaTypes.ProductGallery)),
                Attributes = product.AttributeValues.Select(av => new GetProductAttributeValueDTO
                {
                    AttributeId = av.AttributeId,
                    AttributeName = av.Attribute.Name,
                    Value = av.Value
                }).ToList(),
                Variants = variants.Select(v =>
                {
                    v.BasketQuantity = basketQuantities.VariantQuantities.TryGetValue(v.Id, out var q) ? q : 0;
                    return v;
                }).ToList(),
                AvailableOptions = availableOptions,
                UserInfo = new ProductUserInfoDTO
                {
                    UserId = product.CreatedByUserId,
                    UserName = product.CreatedByUser?.UserName,
                    FullName = product.CreatedByUser?.FullName
                }
            };
            return ServiceResult.Ok(result);
        }

        private async Task<ProductBasketQuantities> ProcessUserBasketQuantity(Product product, List<GetProductVariantDTO> variants)
        {
            var currentUserId = accessor.HttpContext.User.Identity.GetUserId<long>();
            var basketResult = await queryDispatcher.SendAsync(new GetOrCreateBasketQuery(currentUserId));

            var result = new ProductBasketQuantities();

            if (!basketResult.IsSuccess)
                return result;

            var basket = basketResult.Data;

            if (product.Type == ProductType.Simple)
            {
                result.ProductQuantity = basket.Items
                    .Where(t => t.ObjectId == product.Id && t.ItemType == BasketItemType.Product)
                    .Sum(t => t.Quantity);
            }
            else if (product.Type == ProductType.Variant)
            {
                var variantIds = variants.Select(v => v.Id).ToList();
                result.VariantQuantities = basket.Items
                    .Where(t => variantIds.Contains(t.ObjectId) && t.ItemType == BasketItemType.Variant)
                    .GroupBy(t => t.ObjectId)
                    .ToDictionary(g => g.Key, g => g.Sum(i => i.Quantity));
            }

            return result;
        }

        public class ProductBasketQuantities
        {
            public int ProductQuantity { get; set; }
            public Dictionary<long, int> VariantQuantities { get; set; } = new();
        }
        private List<GetProductVariantDTO> ProcessVariants(ICollection<ProductVariant> variants)
        {
            return variants.Where(v => !v.IsDeleted).Select(v => new GetProductVariantDTO
            {
                Id = v.Id,
                SKU = v.SKU,
                Price = v.Price,
                AttributeValues = v.AttributeValues.Select(vav => new GetVariantAttributeValueDTO
                {
                    VariantId = vav.VariantId,
                    AttributeId = vav.AttributeId,
                    AttributeName = vav.Attribute.Name,
                    Value = vav.Value
                }).ToList(),
                Attributes = v.AttributeValues.ToDictionary(
                    av => av.Attribute.Name,
                    av => av.Value),
                ProductVariantImages = GetProductImageDTO.Create(
                    mediaService.GetObjectMedia(v.Id, MediaTypes.ProductVariantMain),
                    mediaService.GetObjectMedias(v.Id, MediaTypes.ProductVariantGallery))
            }).ToList();
        }

        private AvailableOptionsDTO ExtractAvailableOptions(List<GetProductVariantDTO> variants)
        {
            var colors = new HashSet<string>();
            var sizes = new HashSet<string>();

            foreach (var variant in variants)
            {
                if (variant.Attributes.TryGetValue("رنگ", out var color))
                    colors.Add(color);

                if (variant.Attributes.TryGetValue("سایز", out var size))
                    sizes.Add(size);
            }

            return new AvailableOptionsDTO
            {
                Colors = colors.ToList(),
                Sizes = sizes.ToList()
            };
        }
    }
}