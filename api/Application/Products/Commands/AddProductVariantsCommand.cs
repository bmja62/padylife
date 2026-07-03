using Application.Cqrs.Commands;
using Application.Products.DTOs;
using Data.Contracts;
using Entities.Products;
using Services;

namespace Application.Products.Commands
{
    public class AddProductVariantsCommand : ICommand<ServiceResult>
    {
        public AddProductVariantsCommand(long productId, List<CreateProductVariantDTO> variants)
        {
            ProductId = productId;
            Variants = variants;
        }

        public long ProductId { get; }
        public List<CreateProductVariantDTO> Variants { get; }
    }

    public class AddProductVariantsCommandHandler : ICommandHandler<AddProductVariantsCommand, ServiceResult>
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<ProductVariant> _variantRepo;
        private readonly IRepository<VariantAttributeValue> _variantAttrRepo;

        public AddProductVariantsCommandHandler(
            IRepository<Product> productRepo,
            IRepository<ProductVariant> variantRepo,
            IRepository<VariantAttributeValue> variantAttrRepo)
        {
            _productRepo = productRepo;
            _variantRepo = variantRepo;
            _variantAttrRepo = variantAttrRepo;
        }

        public async Task<ServiceResult> Handle(AddProductVariantsCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepo.GetByIdAsync(cancellationToken, request.ProductId);
            if (product == null)
                return ServiceResult.Fail("محصول یافت نشد");

            if (product.Type != ProductType.Variant)
                return ServiceResult.Fail("این محصول از نوع متغیر نیست");



            foreach (var variantDto in request.Variants)
            {
                var variant = new ProductVariant
                {
                    ProductId = product.Id,
                    SKU = variantDto.SKU,
                    Price = variantDto.Price,
                };

                await _variantRepo.AddAsync(variant, cancellationToken);

                foreach (var attrValue in variantDto.AttributeValues)
                {
                    await _variantAttrRepo.AddAsync(new VariantAttributeValue
                    {
                        VariantId = variant.Id,
                        AttributeId = attrValue.AttributeId,
                        Value = attrValue.Value
                    }, cancellationToken);
                }
            }

            await _productRepo.UpdateAsync(product, cancellationToken);

            return ServiceResult.Ok();
        }
    }

}
