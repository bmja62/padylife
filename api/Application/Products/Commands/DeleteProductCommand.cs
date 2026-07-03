using Application.Cqrs.Commands;
using Common.Utilities;
using Data.Contracts;
using Entities.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Products.Commands
{
    public class DeleteProductCommand : ICommand<ServiceResult>
    {
        public DeleteProductCommand(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }

    public class DeleteProductCommandHandler(
        IRepository<Product> productRepository,
        IRepository<ProductVariant> variantRepository,
        IRepository<VariantAttributeValue> variantAttributeValueRepository,
        IRepository<ProductAttributeValue> attributeValueRepository,
        IHttpContextAccessor accessor) : ICommandHandler<DeleteProductCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var userId = accessor.HttpContext.User.Identity.GetUserId<long>();
            if (userId <= 0)
                return ServiceResult.Fail("شناسه کاربری یافت نشد");

            // دریافت محصول به همراه روابطش
            var product = await productRepository.Table
                .Include(p => p.AttributeValues)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.AttributeValues)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (product == null)
                return ServiceResult.Fail("محصول یافت نشد");

            // حذف VariantAttributeValues (ویژگی‌های variants)
            foreach (var variant in product.Variants)
            {
                foreach (var attrValue in variant.AttributeValues.ToList())
                {
                    await variantAttributeValueRepository.DeleteAsync(attrValue, cancellationToken);
                }
            }

            // حذف ProductVariants
            foreach (var variant in product.Variants.ToList())
            {
                await variantRepository.DeleteAsync(variant, cancellationToken);
            }

            // حذف ProductAttributeValues (ویژگی‌های عمومی محصول)
            foreach (var attrValue in product.AttributeValues.ToList())
            {
                await attributeValueRepository.DeleteAsync(attrValue, cancellationToken);
            }

            // حذف نهایی محصول
            await productRepository.DeleteAsync(product, cancellationToken);

            return ServiceResult.Ok();


        }
    }
}