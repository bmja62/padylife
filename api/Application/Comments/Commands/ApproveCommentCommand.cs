using Application.Cqrs.Commands;
using Services;
using Services.Services.CommentServices;

namespace Application.Comments.Commands
{
    // ApproveCommentCommand
    public class ApproveCommentCommand(long commentId) : ICommand<ServiceResult>
    {
        public long CommentId { get; } = commentId;
    }

    public class ApproveCommentCommandHandler(ICommentService commentService) : ICommandHandler<ApproveCommentCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(ApproveCommentCommand request, CancellationToken cancellationToken)
        {
            var success = await commentService.ApproveCommentAsync(request.CommentId);
            return success ? ServiceResult.Ok() : ServiceResult.Fail("نظر یافت نشد");
        }
    }
}
