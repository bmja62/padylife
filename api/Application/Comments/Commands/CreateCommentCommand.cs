using Application.Cqrs.Commands;
using Common.Utilities;
using Microsoft.AspNetCore.Http;
using Services;
using Services.Services.CommentServices;
using Services.Services.CommentServices.DTOs;

namespace Application.Comments.Commands
{
    // CreateCommentCommand
    public class CreateCommentCommand(CreateCommentDTO input) : ICommand<ServiceResult>
    {
        public CreateCommentDTO Input { get; } = input;
    }

    public class CreateCommentCommandHandler(ICommentService commentService, IHttpContextAccessor accessor) : ICommandHandler<CreateCommentCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            var userId = accessor.HttpContext.User.Identity.GetUserId<long>();
            if (userId <= 0) return ServiceResult.Fail("شناسه کاربر نامعتبر است");

            await commentService.AddCommentAsync(userId, request.Input);
            return ServiceResult.Ok();
        }
    }

}
