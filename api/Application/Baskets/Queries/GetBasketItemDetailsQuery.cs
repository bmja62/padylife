using Application.Baskets.DTOs;
using Application.Cqrs.Queris;
using Application.Products.DTOs;
using Common.Utilities;
using Data.Contracts;
using Entities.Baskets;
using Entities.Medias;
using Entities.Products;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Services.MediaServices;

namespace Application.Baskets.Queries
{
    public record GetBasketItemDetailsQuery(List<BasketItemRequestDTO> Items) : IQuery<ServiceResult<List<BasketItemDetailsDTO>>>;

    public class GetBasketItemDetailsQueryHandler
        (
        IRepository<Product> productRepository,
        IRepository<ProductVariant> productVariantRepository,
        IMediaService mediaService
        ) : IQueryHandler<GetBasketItemDetailsQuery, ServiceResult<List<BasketItemDetailsDTO>>>
    {
        public async Task<ServiceResult<List<BasketItemDetailsDTO>>> Handle(GetBasketItemDetailsQuery request, CancellationToken cancellationToken)
        {
            var result = new List<BasketItemDetailsDTO>();

            foreach (var item in request.Items)
            {
                switch (item.ItemType)
                {
                    case BasketItemType.Product:
                        var product = await productRepository.Table.Include(t => t.Category).Where(t => t.Id == item.ObjectId).FirstOrDefaultAsync(cancellationToken);
                        if (product != null)
                        {
                            result.Add(new BasketItemDetailsDTO
                            {
                                Id = product.Id,
                                Title = product.Name,
                                Type = product.Type.ToDisplay(DisplayProperty.Name),
                                UnitPrice = product.Price,
                                Quantity = item.Quantity,
                                Brand = product.Category?.Name,
                                ImageUrl = GetProductImageDTO.Create(
                                                    mediaService.GetObjectMedia(product.Id, MediaTypes.ProductMain),
                                                    mediaService.GetObjectMedias(product.Id, MediaTypes.ProductGallery)), // fetch from file system or image entity
                            });
                        }
                        break;

                    case BasketItemType.Variant:
                        var variant = await productVariantRepository.Table
                            .Include(t => t.Product)
                                .ThenInclude(t => t.Category)
                            .Include(t => t.AttributeValues)
                                .ThenInclude(t => t.Attribute)
                                .Where(t => t.Id == item.ObjectId).FirstOrDefaultAsync(cancellationToken);
                        if (variant != null)
                        {
                            result.Add(new BasketItemDetailsDTO
                            {
                                Id = variant.Id,
                                Title = variant.Product.Name,
                                Type = variant.Product.Type.ToDisplay(DisplayProperty.Name),
                                UnitPrice = variant.Price,
                                Quantity = item.Quantity,
                                Brand = variant.Product.Category?.Name,
                                VariantAttributes = variant.GetVariantIdentifier(),
                                ImageUrl = GetProductImageDTO.Create(
                                            mediaService.GetObjectMedia(variant.Id, MediaTypes.ProductVariantMain),
                                            mediaService.GetObjectMedias(variant.Id, MediaTypes.ProductVariantGallery))
                            });
                        }
                        break;
                }
            }

            return ServiceResult.Ok(result);
        }
    }
}
