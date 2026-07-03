using Application.Cqrs.Commands;
using Data.Contracts;
using Entities.UploadedFiles;
using Microsoft.AspNetCore.Http;
using Services;
using Services.Uploader;
using Services.Uploader.DTO;

namespace Application.UploadedFiles.Commands.UploadAsWebp;

public class UploadAsWebpCommandHandler(IRepository<UploadedFile> repository, IHttpContextAccessor httpContext, IUploaderService uploaderService) : ICommandHandler<UploadAsWebpCommand, ServiceResult<UploadResultDTO>>
{
    public async Task<ServiceResult<UploadResultDTO>> Handle(UploadAsWebpCommand request, CancellationToken cancellationToken) =>
        await uploaderService.UploadAsWebpV2(new UploadV2DTO
        {
            File = request.File,
            FileType = request.FileType,
            Title = request.Title,
            Alt = request.Alt,
            Description = request.Description
        });


}
