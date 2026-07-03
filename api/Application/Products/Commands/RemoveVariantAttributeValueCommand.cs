using Application.Cqrs.Commands;
using Application.Products.DTOs;
using Data.Contracts;
using Entities.Products;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Products.Commands
{
    public class RemoveVariantAttributeValueCommand : ICommand<ServiceResult>
    {
        public RemoveVariantAttributeValueDTO Input { get; }

        public RemoveVariantAttributeValueCommand(RemoveVariantAttributeValueDTO input)
        {
            Input = input;
        }
    }

    public class RemoveVariantAttributeValueCommandHandler :
        ICommandHandler<RemoveVariantAttributeValueCommand, ServiceResult>
    {
        private readonly IRepository<VariantAttributeValue> _variantAttrValueRepo;

        public RemoveVariantAttributeValueCommandHandler(
            IRepository<VariantAttributeValue> variantAttrValueRepo)
        {
            _variantAttrValueRepo = variantAttrValueRepo;
        }

        public async Task<ServiceResult> Handle(
            RemoveVariantAttributeValueCommand request,
            CancellationToken cancellationToken)
        {
            var attributeValue = await _variantAttrValueRepo.Table
                .Where(t =>
                    t.VariantId == request.Input.VariantId &&
                    t.AttributeId == request.Input.AttributeId)
                .FirstOrDefaultAsync(cancellationToken);

            if (attributeValue != null)
            {
                await _variantAttrValueRepo.DeleteAsync(attributeValue, cancellationToken);
            }

            return ServiceResult.Ok();
        }
    }
}