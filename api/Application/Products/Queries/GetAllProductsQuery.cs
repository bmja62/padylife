using Application.Cqrs.Queris;
using Application.Products.DTOs;
using Common.GridResults;
using Data.Contracts;
using Data.Repositories.Extentions;
using Entities.Medias;
using Entities.Products;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Services.MediaServices;
using Services.Services.StockManagerServices;

namespace Application.Products.Queries
{
    public class GetAllProductsQuery : IQuery<ServiceResult<GlobalGridResult<GetAllProductsDTO>>>
    {
        public GetAllProductsQuery(GetAllProductsQueryDTO input)
        {
            Input = input;
        }

        public GetAllProductsQueryDTO Input { get; }
    }

    public class GetAllProductsQueryHandler(
        IRepository<Product> productRepository, IMediaService mediaService, IStockManagerService stockManagerService) : IQueryHandler<GetAllProductsQuery, ServiceResult<GlobalGridResult<GetAllProductsDTO>>>
    {
        public async Task<ServiceResult<GlobalGridResult<GetAllProductsDTO>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var query = productRepository.TableNoTracking
                .Include(p => p.Category)
                .Include(p => p.AttributeValues)
                    .ThenInclude(av => av.Attribute)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.AttributeValues)
                    .ThenInclude(vav => vav.Attribute)
                .AsQueryable();

            // فیلترها
            if (request.Input.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == request.Input.CategoryId);

            if (!string.IsNullOrEmpty(request.Input.SearchTerm))
                query = query.Where(p => p.Name.Contains(request.Input.SearchTerm) ||
                                       p.Description.Contains(request.Input.SearchTerm));

            var data = await query
                .ApplyPagination(request.Input.PageNumber.Value, request.Input.Count.Value)
                .Select(p => new GetAllProductsDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    Type = p.Type,
                    ProductImages = GetProductImageDTO.Create(mediaService.GetObjectMedia(p.Id, MediaTypes.ProductMain), mediaService.GetObjectMedias(p.Id, MediaTypes.ProductGallery)),
                    Attributes = p.AttributeValues.Select(av => new GetProductAttributeValueDTO
                    {
                        AttributeId = av.AttributeId,
                        AttributeName = av.Attribute.Name,
                        Value = av.Value
                    }).ToList(),
                    Variants = p.Variants.Select(v => new GetProductVariantDTO
                    {
                        Id = v.Id,
                        SKU = v.SKU,
                        Price = v.Price,
                        ProductVariantImages = GetProductImageDTO.Create(mediaService.GetObjectMedia(v.Id, MediaTypes.ProductVariantMain), mediaService.GetObjectMedias(v.Id, MediaTypes.ProductVariantGallery)),
                        AttributeValues = v.AttributeValues.Select(vav => new GetVariantAttributeValueDTO
                        {
                            AttributeId = vav.AttributeId,
                            AttributeName = vav.Attribute.Name,
                            Value = vav.Value
                        }).ToList()
                    }).ToList()
                })
                .ToListAsync(cancellationToken);


            var totalCount = await query.CountAsync(cancellationToken);

            return ServiceResult.Ok(new GlobalGridResult<GetAllProductsDTO>
            {
                Data = data,
                TotalCount = totalCount
            });
        }
    }
}