using Application.Cqrs.Commands;
using Application.Products.DTOs;
using Data.Contracts;
using Entities.Products;
using Services;

namespace Application.Products.Commands
{
    public class AddProductAttributeValueCommand : ICommand<ServiceResult>
    {
        public AddProductAttributeValueCommand(AddProductAttributeValueDTO input)
        {
            Input = input;
        }

        public AddProductAttributeValueDTO Input { get; }
    }

    public class AddProductAttributeValueCommandHandler(IRepository<ProductAttributeValue> attributeValueRepository) : ICommandHandler<AddProductAttributeValueCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(AddProductAttributeValueCommand request, CancellationToken cancellationToken)
        {
            await attributeValueRepository.AddAsync(new ProductAttributeValue
            {
                ProductId = request.Input.ProductId,
                AttributeId = request.Input.AttributeId,
                Value = request.Input.Value,
            }, cancellationToken);
            return ServiceResult.Ok();
        }
    }
}
