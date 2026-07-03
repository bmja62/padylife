//using Application.Cqrs.Commands;
//using Application.Products.DTOs;
//using Application.Uploaders.Command;
//using Common.Utilities;
//using Data.Repositories;
//using Entities.Blogs;
//using Entities.Products;
//using Entities.UploadedFiles;
//using Microsoft.AspNetCore.Http;
//using Services;
//using System.Threading;

//namespace Application.Products.Commands
//{
//    public class CreateProductCommand : ICommand<ServiceResult>
//    {
//        public CreateProductCommand(CreateProductCommandDTO input)
//        {
//            Input = input;
//        }

//        public CreateProductCommandDTO Input { get; }
//    }

//    public class CreateProductCommandHandler(
//        ICommandDispatcher commandDispatcher,
//        IRepository<Product> productRepository,
//        IRepository<ProductAttributeValue> attributeValueRepository,
//        IRepository<ProductVariant> variantRepository,
//        IRepository<VariantAttributeValue> variantAttributeValueRepository,
//        IHttpContextAccessor accessor) : ICommandHandler<CreateProductCommand, ServiceResult>
//    {
//        public async Task<ServiceResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
//        {
//            var userId = accessor.HttpContext.User.Identity.GetUserId<long>();
//            if (userId <= 0)
//                return ServiceResult.Fail("شناسه کاربری یافت نشد");

//            // ایجاد محصول اصلی
//            var product = new Product
//            {
//                Name = request.Input.Name,
//                Description = request.Input.Description,
//                Price = request.Input.Price,
//                CategoryId = request.Input.CategoryId,
//                CreatedByUserId = userId
//            };

//            await productRepository.AddAsync(product, cancellationToken);

//            //افزودن تصاویر محصول
//            if (request.Input.ProductImage != null) 
//            {
//                //افزودن تصویر اصلی محصول ساده
//                if (request.Input?.ProductImage?.MainImage != null && request.Input?.ProductImage?.MainImage.Length > 0) 
//                {
//                     await commandDispatcher
//                             .SendAsync(new UploadCommand(request.Input?.ProductImage?.MainImage, product.Id, Entities.Medias.MediaTypes.ProductMain, false, UploadType.ProductMain));
//                }
//                //افزودن تصاویر گالری محصول ساده
//                if (request.Input?.ProductImage?.Gallery != null)
//                {
//                    foreach (var productGalleryImage in request.Input?.ProductImage?.Gallery)
//                    {
//                        if (productGalleryImage != null && productGalleryImage.Length > 0) 
//                        {
//                            await commandDispatcher
//                                    .SendAsync(new UploadCommand(productGalleryImage, product.Id, Entities.Medias.MediaTypes.ProductGallery, false, UploadType.ProductGallery));
//                        }
//                    }
//                }
//            }

//            // افزودن ویژگی‌های عمومی محصول
//            if (request.Input.AttributeValues != null)
//            {
//                foreach (var attrValue in request.Input.AttributeValues)
//                {
//                    await attributeValueRepository.AddAsync(new ProductAttributeValue
//                    {
//                        ProductId = product.Id,
//                        AttributeId = attrValue.AttributeId,
//                        Value = attrValue.Value,

//                    }, cancellationToken);
//                }
//            }

//            // افزودن variants محصول
//            if (request.Input.Variants != null)
//            {
//                foreach (var variantDto in request.Input.Variants)
//                {
//                    var variant = new ProductVariant
//                    {
//                        ProductId = product.Id,
//                        SKU = variantDto.SKU,
//                        Price = variantDto.Price,
//                    };

//                    await variantRepository.AddAsync(variant, cancellationToken);

//                    //افزودن تصاویر محصول متغییر
//                    if (variantDto.ProductVariantImage != null)
//                    {
//                        //افزودن تصویر اصلی محصول متغییر
//                        if (variantDto.ProductVariantImage?.MainImage != null && variantDto.ProductVariantImage?.MainImage.Length > 0)
//                        {
//                            await commandDispatcher
//                                    .SendAsync(new UploadCommand(variantDto.ProductVariantImage.MainImage, variant.Id, Entities.Medias.MediaTypes.ProductVariantMain, false, UploadType.ProductVariantMain));
//                        }
//                        //افزودن تصاویر گالری محصول متغییر
//                        if (variantDto.ProductVariantImage.Gallery != null)
//                        {
//                            foreach (var productVariantGalleryImage in variantDto.ProductVariantImage.Gallery)
//                            {
//                                if (productVariantGalleryImage != null && productVariantGalleryImage.Length > 0)
//                                {
//                                    await commandDispatcher
//                                            .SendAsync(new UploadCommand(productVariantGalleryImage, variant.Id, Entities.Medias.MediaTypes.ProductVariantGallery, false, UploadType.ProductVariantGallery));
//                                }
//                            }
//                        }
//                    }

//                    // افزودن مقادیر ویژگی‌های variant
//                    foreach (var variantAttr in variantDto.AttributeValues)
//                    {
//                        await variantAttributeValueRepository.AddAsync(new VariantAttributeValue
//                        {
//                            VariantId = variant.Id,
//                            AttributeId = variantAttr.AttributeId,
//                            Value = variantAttr.Value,

//                        }, cancellationToken);
//                    }
//                }

//            }

//            return ServiceResult.Ok();
//        }
//    }
//}