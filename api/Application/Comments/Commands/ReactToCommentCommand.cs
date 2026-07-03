using Application.Cqrs.Commands;
using Common.Utilities;
using Entities.Comments;
using Microsoft.AspNetCore.Http;
using Services;
using Services.Services.CommentServices;

namespace Application.Comments.Commands
{
    public record ReactToCommentCommand(long CommentId, ReactionType ReactionType) : ICommand<ServiceResult>;

    public class ReactToCommentCommandHandler(ICommentService commentService, IHttpContextAccessor accessor)
        : ICommandHandler<ReactToCommentCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(ReactToCommentCommand request, CancellationToken cancellationToken)
        {
            var userId = accessor.HttpContext.User.Identity.GetUserId<long>();
            if (userId <= 0)
                return ServiceResult.Fail("شناسه کاربر معتبر نیست.");

            await commentService.ReactToCommentAsync(request.CommentId, userId, request.ReactionType);
            return ServiceResult.Ok();
        }
    }
}
