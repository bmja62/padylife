using Application.Cqrs.Commands;
using Services;
using Services.Uploader;

namespace Application.Uploaders.Command
{
    public class DeleteFileCommand(string guid) : ICommand<ServiceResult>
    {
        public string Guid { get; } = guid;
    }

    public class DeleteFileCommandHandler(IUploaderService uploaderService) : ICommandHandler<DeleteFileCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(DeleteFileCommand request, CancellationToken cancellationToken) =>
         await uploaderService.DeleteFile(request.Guid);
    }
}
