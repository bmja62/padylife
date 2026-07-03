using Application.Cqrs.Commands;
using Application.Products.DTOs;
using Common.Utilities;
using Data.Contracts;
using Entities.Products;
using Microsoft.AspNetCore.Http;
using Services;

namespace Application.Products.Commands
{
    public class CreateProductBasicCommand(CreateProductBasicInfoDTO input) : ICommand<ServiceResult<GetProductByIdDTO>>
    {
        public CreateProductBasicInfoDTO Input { get; } = input;
    }

    public class CreateProductBasicCommandHandler : ICommandHandler<CreateProductBasicCommand, ServiceResult<GetProductByIdDTO>>
    {
        private readonly IRepository<Product> _productRepo;
        private readonly IHttpContextAccessor _accessor;

        public CreateProductBasicCommandHandler(
            IRepository<Product> productRepo,
            IHttpContextAccessor accessor)
        {
            _productRepo = productRepo;
            _accessor = accessor;
        }

        public async Task<ServiceResult<GetProductByIdDTO>> Handle(CreateProductBasicCommand request, CancellationToken cancellationToken)
        {
            var userId = _accessor.HttpContext.User.Identity.GetUserId<long>();
            if (userId <= 0)
                return ServiceResult.BadRequest<GetProductByIdDTO>("شناسه کاربری یافت نشد");

            var product = new Product
            {
                Name = request.Input.Name,
                Description = request.Input.Description,
                CategoryId = request.Input.CategoryId,
                Type = request.Input.Type,
                CreatedByUserId = userId
            };

            await _productRepo.AddAsync(product, cancellationToken);

            return ServiceResult.Ok(new GetProductByIdDTO
            {
                Id = product.Id,
            });
        }
    }

}
