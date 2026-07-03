using Application.Cqrs.Commands;
using Application.Products.DTOs;
using Common.Utilities;
using Data.Contracts;
using Entities.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Products.Commands
{
    public class UpdateProductCommand : ICommand<ServiceResult>
    {
        public UpdateProductCommand(long id, UpdateProductCommandDTO input)
        {
            Id = id;
            Input = input;
        }

        public long Id { get; }
        public UpdateProductCommandDTO Input { get; }
    }

    public class UpdateProductCommandHandler(
        IRepository<Product> productRepository,
        IRepository<ProductAttributeValue> attributeValueRepository,
        IRepository<ProductVariant> variantRepository,
        IRepository<VariantAttributeValue> variantAttributeValueRepository,
        IHttpContextAccessor accessor) : ICommandHandler<UpdateProductCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var userId = accessor.HttpContext.User.Identity.GetUserId<long>();
            if (userId <= 0)
                return ServiceResult.Fail("شناسه کاربری یافت نشد");

            // دریافت محصول موجود
            var product = await productRepository.Table
                .Include(p => p.AttributeValues)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.AttributeValues)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (product == null)
                return ServiceResult.Fail("محصول یافت نشد");

            // به‌روزرسانی اطلاعات پایه محصول
            product.Name = request.Input.Name;
            product.Description = request.Input.Description;
            product.Price = request.Input.Price;
            product.CategoryId = request.Input.CategoryId;
            product.UpdatedAt = DateTime.Now;

            // مدیریت ویژگی‌های عمومی محصول
            await UpdateProductAttributes(product, request.Input.AttributeValues, userId, cancellationToken);

            // مدیریت variants محصول
            await UpdateProductVariants(product, request.Input.Variants, userId, cancellationToken);



            await productRepository.UpdateAsync(product, cancellationToken);

            return ServiceResult.Ok();
        }

        private async Task UpdateProductAttributes(
            Product product,
            List<ProductAttributeValueDTO> newAttributeValues,
            long userId,
            CancellationToken cancellationToken)
        {
            // حذف ویژگی‌های قدیمی
            foreach (var existingAttr in product.AttributeValues.ToList())
            {
                await attributeValueRepository.DeleteAsync(existingAttr, cancellationToken);
            }

            // افزودن ویژگی‌های جدید
            if (newAttributeValues != null)
            {
                foreach (var attrValue in newAttributeValues)
                {
                    await attributeValueRepository.AddAsync(new ProductAttributeValue
                    {
                        ProductId = product.Id,
                        AttributeId = attrValue.AttributeId,
                        Value = attrValue.Value
                    }, cancellationToken);
                }
            }
        }

        private async Task UpdateProductVariants(
            Product product,
            List<UpdateProductVariantDTO> newVariants,
            long userId,
            CancellationToken cancellationToken)
        {
            // حذف variants قدیمی
            foreach (var existingVariant in product.Variants.ToList())
            {
                // حذف مقادیر ویژگی‌های variant
                foreach (var attrValue in existingVariant.AttributeValues.ToList())
                {
                    await variantAttributeValueRepository.DeleteAsync(attrValue, cancellationToken);
                }

                await variantRepository.DeleteAsync(existingVariant, cancellationToken);
            }

            // افزودن variants جدید
            if (newVariants != null)
            {
                foreach (var variantDto in newVariants)
                {
                    var variant = new ProductVariant
                    {
                        ProductId = product.Id,
                        SKU = variantDto.SKU,
                        Price = variantDto.Price
                    };

                    await variantRepository.AddAsync(variant, cancellationToken);

                    // افزودن مقادیر ویژگی‌های variant
                    foreach (var variantAttr in variantDto.AttributeValues)
                    {
                        await variantAttributeValueRepository.AddAsync(new VariantAttributeValue
                        {
                            VariantId = variant.Id,
                            AttributeId = variantAttr.AttributeId,
                            Value = variantAttr.Value
                        }, cancellationToken);
                    }
                }
            }
        }
    }
}