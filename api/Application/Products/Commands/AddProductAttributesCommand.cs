using Application.Cqrs.Commands;
using Application.Products.DTOs;
using Data.Contracts;
using Entities.Products;
using Services;

namespace Application.Products.Commands
{
    public class AddProductAttributesCommand : ICommand<ServiceResult>
    {
        public AddProductAttributesCommand(long productId, List<ProductAttributeValueDTO> attributeValues)
        {
            ProductId = productId;
            AttributeValues = attributeValues;
        }

        public long ProductId { get; }
        public List<ProductAttributeValueDTO> AttributeValues { get; }
    }

    public class AddProductAttributesCommandHandler : ICommandHandler<AddProductAttributesCommand, ServiceResult>
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<ProductAttribute> _attributeRepo;
        private readonly IRepository<ProductAttributeValue> _attrValueRepo;

        public AddProductAttributesCommandHandler(
            IRepository<Product> productRepo,
            IRepository<ProductAttribute> attributeRepo,
            IRepository<ProductAttributeValue> attrValueRepo)
        {
            _productRepo = productRepo;
            _attributeRepo = attributeRepo;
            _attrValueRepo = attrValueRepo;
        }

        public async Task<ServiceResult> Handle(AddProductAttributesCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepo.GetByIdAsync(cancellationToken, request.ProductId);
            if (product == null)
                return ServiceResult.Fail("محصول مورد نظر یافت نشد");



            // حذف ویژگی‌های قبلی اگر وجود داشته باشند
            var existingValues = _attrValueRepo.Table.Where(x => x.ProductId == request.ProductId).ToList();
            if (existingValues.Any())
            {
                await _attrValueRepo.DeleteRangeAsync(existingValues, cancellationToken);
            }

            // افزودن ویژگی‌های جدید
            foreach (var attrValue in request.AttributeValues)
            {
                var attribute = _attributeRepo.Table.FirstOrDefault(a => a.Id == attrValue.AttributeId);
                if (attribute == null)
                {
                    attribute = new ProductAttribute
                    {
                        Name = attribute.Name,
                        Type = AttributeType.Text,
                    };
                    await _attributeRepo.AddAsync(attribute, cancellationToken);
                }

                await _attrValueRepo.AddAsync(new ProductAttributeValue
                {
                    ProductId = request.ProductId,
                    AttributeId = attribute.Id,
                    Value = attrValue.Value
                }, cancellationToken);
            }


            await _productRepo.UpdateAsync(product, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
