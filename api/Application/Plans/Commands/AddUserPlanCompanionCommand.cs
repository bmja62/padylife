using Application.Chats.Events;
using Application.Cqrs.Commands;
using Application.Plans.DTOs;
using Common.Utilities;
using Data.Contracts;
using Entities.Chats;
using Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Commands
{
    public class AddUserPlanCompanionCommand : ICommand<ServiceResult>
    {
        public AddUserPlanCompanionCommand(AddUserPlanCompanionCommandDTO input)
        {
            Input = input;
        }

        public AddUserPlanCompanionCommandDTO Input { get; }
    }

    public class AddUserPlanCompanionHandler(
        IRepository<UserPlanCompanion> companionRepo,
        IRepository<UserPlan> planRepo,
        IRepository<ChatRoom> chatRoomRepo,
        IHttpContextAccessor accessor
    ) : ICommandHandler<AddUserPlanCompanionCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(AddUserPlanCompanionCommand request, CancellationToken cancellationToken)
        {
            var userId = accessor.HttpContext.User.Identity.GetUserId<long>();

            var userPlan = await planRepo.GetByIdAsync(cancellationToken, request.Input.UserPlanId);
            if (userPlan == null || userPlan.UserId != userId)
                return ServiceResult.Fail("برنامه موردنظر یافت نشد یا متعلق به شما نیست.");

            if (await companionRepo.Table.Where(t => t.UserPlanId == request.Input.UserPlanId && t.CompanionUserId == request.Input.CompanionUserId).AnyAsync(cancellationToken))
                return ServiceResult.BadRequest("شما قبلا ایشان را به عنوان همراه به این پلن اضافه کرده اید.");

            var companion = UserPlanCompanion.Join(
                userPlanId: request.Input.UserPlanId,
                companionUserId: request.Input.CompanionUserId
            );
            var companionUserId = request.Input.CompanionUserId;
            await companionRepo.AddAsync(companion, cancellationToken);

            // بررسی چت قبلی
            var existingChat = await chatRoomRepo.Table
                .Include(t => t.Participants)
                .FirstOrDefaultAsync(c =>
                    !c.IsGroup &&
                    c.Participants.Any(p => p.UserId == userId) &&
                    c.Participants.Any(p => p.UserId == companionUserId),
                    cancellationToken);

            if (existingChat == null)
            {
                var chatRoom = ChatRoom.CreatePrivateChat(userId, companionUserId);
                await chatRoomRepo.AddAsync(chatRoom, cancellationToken);
                chatRoom.AddDomainEvent(new NewChatRoomEvent([userId, companionUserId]));
            }

            return ServiceResult.Ok("همراه با موفقیت افزوده شد و چت ایجاد گردید.");
        }
    }
}
