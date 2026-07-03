using Application.Cqrs.Commands;
using Application.Products.DTOs;
using Application.Uploaders.Command;
using Data.Contracts;
using Entities.Products;
using Entities.UploadedFiles;
using Services;

namespace Application.Products.Commands
{
    public class AddProductMediaCommand : ICommand<ServiceResult>
    {
        public AddProductMediaCommand(long productId, AddProductMediaDTO input)
        {
            ProductId = productId;
            Input = input;
        }

        public long ProductId { get; }
        public AddProductMediaDTO Input { get; }
    }

    public class AddProductMediaCommandHandler : ICommandHandler<AddProductMediaCommand, ServiceResult>
    {
        private readonly IRepository<Product> _productRepo;
        private readonly ICommandDispatcher _commandDispatcher;

        public AddProductMediaCommandHandler(
            IRepository<Product> productRepo,
            ICommandDispatcher commandDispatcher)
        {
            _productRepo = productRepo;
            _commandDispatcher = commandDispatcher;
        }

        public async Task<ServiceResult> Handle(AddProductMediaCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepo.GetByIdAsync(cancellationToken, request.ProductId);
            if (product == null)
                return ServiceResult.Fail("محصول یافت نشد");


            // آپلود تصویر اصلی
            if (request.Input.MainImage != null)
            {
                var uploadResult = await _commandDispatcher.SendAsync(new UploadCommand(
                    request.Input.MainImage,
                    product.Id,
                    Entities.Medias.MediaTypes.ProductMain,
                    false,
                    UploadType.ProductMain
                ));
                if (!uploadResult.IsSuccess)
                    return ServiceResult.Fail("خطا در آپلود تصاویر گالری");
            }

            // آپلود تصاویر گالری
            if (request.Input.GalleryImages != null)
            {
                foreach (var image in request.Input.GalleryImages)
                {
                    var uploadResult = await _commandDispatcher.SendAsync(new UploadCommand(
                        image,
                        product.Id,
                        Entities.Medias.MediaTypes.ProductGallery,
                        false,
                        UploadType.ProductGallery
                    ));

                    if (!uploadResult.IsSuccess)
                        return ServiceResult.Fail("خطا در آپلود تصاویر گالری");
                }
            }

            await _productRepo.UpdateAsync(product, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}