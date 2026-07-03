using Application.Cqrs.Commands;
using Common.Utilities;
using Microsoft.AspNetCore.Http;
using Services;
using Services.Services.CommentServices;
using Services.Services.CommentServices.DTOs;

namespace Application.Comments.Commands
{
    // ReplyCommentCommand
    public class ReplyCommentCommand(CreateCommentDTO input) : ICommand<ServiceResult>
    {
        public CreateCommentDTO Input { get; } = input;
    }

    public class ReplyCommentCommandHandler(ICommentService commentService, IHttpContextAccessor accessor) : ICommandHandler<ReplyCommentCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(ReplyCommentCommand request, CancellationToken cancellationToken)
        {
            var userId = accessor.HttpContext.User.Identity.GetUserId<long>();
            if (userId <= 0) return ServiceResult.Fail("شناسه کاربر نامعتبر است");

            await commentService.AddCommentAsync(userId, request.Input);
            return ServiceResult.Ok();
        }
    }
}
