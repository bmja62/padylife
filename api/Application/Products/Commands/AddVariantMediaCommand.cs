using Application.Cqrs.Commands;
using Application.Products.DTOs;
using Application.Uploaders.Command;
using Data.Contracts;
using Entities.Products;
using Entities.UploadedFiles;
using Services;

namespace Application.Products.Commands
{
    public class AddVariantMediaCommand : ICommand<ServiceResult>
    {
        public AddVariantMediaCommand(long variantId, ProductVariantImageDTO media)
        {
            VariantId = variantId;
            Media = media;
        }

        public long VariantId { get; }
        public ProductVariantImageDTO Media { get; }
    }

    public class AddVariantMediaCommandHandler : ICommandHandler<AddVariantMediaCommand, ServiceResult>
    {
        private readonly IRepository<ProductVariant> _variantRepo;
        private readonly ICommandDispatcher _commandDispatcher;

        public AddVariantMediaCommandHandler(
            IRepository<ProductVariant> variantRepo,
            ICommandDispatcher commandDispatcher)
        {
            _variantRepo = variantRepo;
            _commandDispatcher = commandDispatcher;
        }

        public async Task<ServiceResult> Handle(AddVariantMediaCommand request, CancellationToken cancellationToken)
        {
            var variant = await _variantRepo.GetByIdAsync(cancellationToken, request.VariantId);
            if (variant == null)
                return ServiceResult.Fail("واریانت یافت نشد");

            // آپلود تصویر اصلی
            if (request.Media.MainImage != null)
            {
                var uploadResult = await _commandDispatcher.SendAsync(new UploadCommand(
                    request.Media.MainImage,
                    variant.Id,
                    Entities.Medias.MediaTypes.ProductVariantMain,
                    false,
                    UploadType.ProductVariantMain
                ));

                if (!uploadResult.IsSuccess)
                    return ServiceResult.Fail("خطا در آپلود تصویر اصلی");
            }

            // آپلود تصاویر گالری
            if (request.Media.Gallery != null)
            {
                foreach (var image in request.Media.Gallery)
                {
                    var uploadResult = await _commandDispatcher.SendAsync(new UploadCommand(
                        image,
                        variant.Id,
                        Entities.Medias.MediaTypes.ProductVariantGallery,
                        false,
                        UploadType.ProductVariantGallery
                    ));

                    if (!uploadResult.IsSuccess)
                        return ServiceResult.Fail("خطا در آپلود تصاویر گالری");
                }
            }


            await _variantRepo.UpdateAsync(variant, cancellationToken);

            return ServiceResult.Ok();
        }
    }
}
