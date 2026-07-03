using Application.Cqrs.Commands;
using Application.Products.DTOs;
using Data.Contracts;
using Entities.Products;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Products.Commands
{
    public class RemoveProductAttributeValueCommand(RemoveProductAttributeValueDTO input) : ICommand<ServiceResult>
    {
        public RemoveProductAttributeValueDTO Input { get; } = input;
    }
    public class RemoveProductAttributeValueCommandHandler(IRepository<ProductAttributeValue> attributeValueRepository) : ICommandHandler<RemoveProductAttributeValueCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(RemoveProductAttributeValueCommand request, CancellationToken cancellationToken)
        {
            var attributeValueInDb = await attributeValueRepository.Table.Where(t => t.ProductId == request.Input.ProductId && t.AttributeId == request.Input.AttributeId).FirstOrDefaultAsync(cancellationToken);
            if (attributeValueInDb != null)
            {
                await attributeValueRepository.DeleteAsync(attributeValueInDb, cancellationToken);
            }
            return ServiceResult.Ok();
        }
    }
}
