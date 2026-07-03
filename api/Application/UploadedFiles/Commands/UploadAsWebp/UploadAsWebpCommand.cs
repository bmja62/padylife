using Application.Cqrs.Commands;
using Entities.UploadedFiles;
using Microsoft.AspNetCore.Http;
using Services;
using Services.Uploader.DTO;

namespace Application.UploadedFiles.Commands.UploadAsWebp;

public class UploadAsWebpCommand(IFormFile file, UploadType fileType, string title, string alt, string description) : ICommand<ServiceResult<UploadResultDTO>>
{
    public IFormFile File { get; set; } = file;
    public UploadType FileType { get; set; } = fileType;
    public string Title { get; set; } = title;
    public string Alt { get; set; } = alt;
    public string Description { get; set; } = description;
}
