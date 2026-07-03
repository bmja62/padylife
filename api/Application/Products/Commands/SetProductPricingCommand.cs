using Application.Cqrs.Commands;
using Application.Products.DTOs;
using Data.Contracts;
using Entities.Products;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Products.Commands
{
    public class SetProductPricingCommand : ICommand<ServiceResult>
    {
        public SetProductPricingCommand(long productId, SetProductPricingDTO input)
        {
            ProductId = productId;
            Input = input;
        }

        public long ProductId { get; }
        public SetProductPricingDTO Input { get; }
    }

    public class SetProductPricingCommandHandler : ICommandHandler<SetProductPricingCommand, ServiceResult>
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<ProductVariant> _variantRepo;

        public SetProductPricingCommandHandler(
            IRepository<Product> productRepo,
            IRepository<ProductVariant> variantRepo)
        {
            _productRepo = productRepo;
            _variantRepo = variantRepo;
        }

        public async Task<ServiceResult> Handle(SetProductPricingCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepo.GetByIdAsync(cancellationToken, request.ProductId);
            if (product == null)
                return ServiceResult.Fail("محصول یافت نشد");



            // تنظیم قیمت پایه
            product.Price = request.Input.BasePrice;
            await _productRepo.UpdateAsync(product, cancellationToken);

            // به‌روزرسانی قیمت واریانت‌ها
            if (product.Type == ProductType.Variant && request.Input.VariantPrices != null)
            {
                foreach (var variantPrice in request.Input.VariantPrices)
                {
                    var variant = await _variantRepo.Table
                        .FirstOrDefaultAsync(v => v.ProductId == product.Id && v.SKU == variantPrice.SKU);

                    if (variant != null)
                    {
                        variant.Price = variantPrice.Price;
                        await _variantRepo.UpdateAsync(variant, cancellationToken);
                    }
                }
            }


            await _productRepo.UpdateAsync(product, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}

