using Application.Cqrs.Commands;
using Application.Products.DTOs;
using Application.Uploaders.Command;
using Entities.UploadedFiles;
using Services;

namespace Application.Products.Commands
{
    public class AddImageToGalleryCommand(AddImageToGalleryDTO input) : ICommand<ServiceResult>
    {
        public AddImageToGalleryDTO Input { get; } = input;
    }

    public class AddImageToGalleryCommandHandler(ICommandDispatcher commandDispatcher) : ICommandHandler<AddImageToGalleryCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(AddImageToGalleryCommand request, CancellationToken cancellationToken)
        {
            if (request.Input.Type == Entities.UploadedFiles.UploadType.ProductGallery)
            {
                //افزودن گالری محصول ساده
                if (request.Input?.GalleryImage != null && request.Input?.GalleryImage.Length > 0)
                {
                    await commandDispatcher
                            .SendAsync(new UploadCommand(request.Input?.GalleryImage, request.Input.ObjectId, Entities.Medias.MediaTypes.ProductGallery, false, UploadType.ProductGallery));
                }
                return ServiceResult.Ok();
            }
            else if (request.Input.Type == Entities.UploadedFiles.UploadType.ProductVariantGallery)
            {
                //افزودن گالری محصول متغییر
                if (request.Input?.GalleryImage != null && request.Input?.GalleryImage.Length > 0)
                {
                    await commandDispatcher
                            .SendAsync(new UploadCommand(request.Input?.GalleryImage, request.Input.ObjectId, Entities.Medias.MediaTypes.ProductVariantGallery, false, UploadType.ProductVariantGallery));
                }
                return ServiceResult.Ok();
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
