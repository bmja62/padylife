using Application.Cqrs.Commands;
using Application.Products.DTOs;
using Application.Uploaders.Command;
using Data.Contracts;
using Entities.Medias;
using Entities.UploadedFiles;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Services.MediaServices;
using Services.Uploader;

namespace Application.Products.Commands
{
    public class ChangeMainImageCommand(ChangeMainImageDTO input) : ICommand<ServiceResult>
    {
        public ChangeMainImageDTO Input { get; } = input;
    }

    public class ChangeMainImageCommandHandler(
        ICommandDispatcher commandDispatcher,
        IRepository<Media> mediaRepository,
        IRepository<UploadedFile> uploadedFileRepository,
        IMediaService mediaService,
        IUploaderService uploaderService
        ) : ICommandHandler<ChangeMainImageCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(ChangeMainImageCommand request, CancellationToken cancellationToken)
        {
            if (request.Input.Type == Entities.UploadedFiles.UploadType.ProductMain)
            {
                //حذف تصاویراصلی محصول ساده قبلی
                var mediaIdsToRemove = (await mediaService.GetObjectMediasAsync(request.Input.ObjectId, MediaTypes.ProductMain)).Select(t => t.Id).ToList();
                var medias = await mediaRepository.Table.Where(t => mediaIdsToRemove.Contains(t.Id)).ToListAsync();
                var guidsToRemove = await uploadedFileRepository.Table.Where(t => medias.Select(t => t.URL).Contains(t.ImageUrl)).Select(t => t.Guid).ToListAsync();
                foreach (var item in guidsToRemove)
                {
                    await uploaderService.DeleteFile(item);
                }
                //افزودن تصویر اصلی محصول ساده
                if (request.Input?.MainImage != null && request.Input?.MainImage.Length > 0)
                {
                    await commandDispatcher
                            .SendAsync(new UploadCommand(request.Input?.MainImage, request.Input.ObjectId, Entities.Medias.MediaTypes.ProductMain, false, UploadType.ProductMain));
                }
                return ServiceResult.Ok();
            }
            else if (request.Input.Type == Entities.UploadedFiles.UploadType.ProductVariantMain)
            {
                //حذف تصاویراصلی محصول متغییر قبلی
                var mediaIdsToRemove = (await mediaService.GetObjectMediasAsync(request.Input.ObjectId, MediaTypes.ProductVariantMain)).Select(t => t.Id).ToList();
                var medias = await mediaRepository.Table.Where(t => mediaIdsToRemove.Contains(t.Id)).ToListAsync();
                var guidsToRemove = await uploadedFileRepository.Table.Where(t => medias.Select(t => t.URL).Contains(t.ImageUrl)).Select(t => t.Guid).ToListAsync();
                foreach (var item in guidsToRemove)
                {
                    await uploaderService.DeleteFile(item);
                }
                //افزودن تصویر اصلی محصول متغییر
                if (request.Input?.MainImage != null && request.Input?.MainImage.Length > 0)
                {
                    await commandDispatcher
                            .SendAsync(new UploadCommand(request.Input?.MainImage, request.Input.ObjectId, Entities.Medias.MediaTypes.ProductVariantMain, false, UploadType.ProductVariantMain));
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
