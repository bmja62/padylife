using Application.Cqrs.Commands;
using Common.Utilities;
using Microsoft.AspNetCore.Http;
using Services;
using Services.Services.CommentServices;

namespace Application.Comments.Commands
{
    // DeleteCommentCommand
    public class DeleteCommentCommand(long commentId) : ICommand<ServiceResult>
    {
        public long CommentId { get; } = commentId;
    }

    public class DeleteCommentCommandHandler(ICommentService commentService, IHttpContextAccessor accessor) : ICommandHandler<DeleteCommentCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var userId = accessor.HttpContext.User.Identity.GetUserId<long>();
            if (userId <= 0) return ServiceResult.Fail("شناسه کاربر نامعتبر است");

            var success = await commentService.DeleteCommentAsync(request.CommentId, userId);
            return success ? ServiceResult.Ok() : ServiceResult.Fail("عدم دسترسی یا نظر یافت نشد");
        }
    }
}
