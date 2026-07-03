using Application.Comments.DTOs;
using Application.Cqrs.Commands;
using Common.Utilities;
using Microsoft.AspNetCore.Http;
using Services;
using Services.Services.CommentServices;

namespace Application.Comments.Commands
{
    // EditCommentCommand
    public class EditCommentCommand(EditCommentDTO input) : ICommand<ServiceResult>
    {
        public EditCommentDTO Input { get; } = input;
    }

    public class EditCommentCommandHandler(ICommentService commentService, IHttpContextAccessor accessor) : ICommandHandler<EditCommentCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(EditCommentCommand request, CancellationToken cancellationToken)
        {
            var userId = accessor.HttpContext.User.Identity.GetUserId<long>();
            if (userId <= 0) return ServiceResult.Fail("شناسه کاربر نامعتبر است");

            await commentService.EditCommentAsync(request.Input.CommentId, userId, request.Input.NewText);
            return ServiceResult.Ok();
        }
    }
}
