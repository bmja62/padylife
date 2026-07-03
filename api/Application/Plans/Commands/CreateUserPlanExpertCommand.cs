using Application.Chats.Events;
using Application.Cqrs.Commands;
using Application.Plans.DTOs;
using Data.Contracts;
using Entities.Chats;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.Plans.Commands
{
    public class CreateUserPlanExpertCommand : ICommand<ServiceResult>
    {
        public CreateUserPlanExpertCommand(CreateUserPlanExpertCommandDTO input)
        {
            Input = input;
        }

        public CreateUserPlanExpertCommandDTO Input { get; }
    }

    public class CreateUserPlanExpertCommandHandler(
       IRepository<UserPlan> userPlanRepo,
       IRepository<UserPlanExpert> userPlanExpertRepo,
       IRepository<ChatRoom> chatRoomRepo,
       IRepository<ChatRoomParticipant> chatParticipantRepo
   ) : ICommandHandler<CreateUserPlanExpertCommand, ServiceResult>
    {
        public async Task<ServiceResult> Handle(CreateUserPlanExpertCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Input;
            var userPlan = await userPlanRepo.GetByIdAsync(cancellationToken, dto.UserPlanId);
            if (userPlan == null)
                return ServiceResult.Fail("پلن کاربر یافت نشد.");

            var userId = userPlan.UserId;
            var expertId = dto.ExpertId;

            // ساخت ارتباط پلن و متخصص
            var assignment = UserPlanExpert.Assign(dto.UserPlanId, expertId, dto.Price, dto.Specialization);
            await userPlanExpertRepo.AddAsync(assignment, cancellationToken);

            // بررسی چت قبلی
            var existingChat = await chatRoomRepo.Table
                .Include(t => t.Participants)
                .FirstOrDefaultAsync(c =>
                    !c.IsGroup &&
                    c.Participants.Any(p => p.UserId == userId) &&
                    c.Participants.Any(p => p.UserId == expertId),
                    cancellationToken);

            if (existingChat == null)
            {
                var chatRoom = ChatRoom.CreatePrivateChat(userId, expertId);
                await chatRoomRepo.AddAsync(chatRoom, cancellationToken);
                chatRoom.AddDomainEvent(new NewChatRoomEvent([userId, expertId]));
            }

            return ServiceResult.Ok("متخصص با موفقیت به پلن اضافه شد.");
        }
    }
}
