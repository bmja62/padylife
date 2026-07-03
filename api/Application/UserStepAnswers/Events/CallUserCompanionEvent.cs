using Application.UserStepAnswers.Extentions;
using Data.Contracts;
using Entities.Common;
using Entities.Common.Events;
using Entities.Excersies;
using Entities.StepOprions;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Services.Services.NotificationServices;

namespace Application.UserStepAnswers.Events
{
    public class CallUserCompanionEvent(long userId, long id, EntityType userPlanExcersiesAnswer, long userPlanId) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public long UserId { get; } = userId;
        public long Id { get; } = id;
        public EntityType UserPlanExcersiesAnswer { get; } = userPlanExcersiesAnswer;
        public long UserPlanId { get; } = userPlanId;
    }

    public class CallUserCompanionEventAction(
        IRepository<UserPlanExcersiesAnswer> userPlanExcersiesAnswerRepository,
        IRepository<StepOption> stepOptionRepository,
        IRepository<Step> stepRepository,
        ISendSystemNotification sendSystemNotification,
        IUserRepository userRepository
        ) : IDomainEventAction<CallUserCompanionEvent>
    {
        private readonly ISendSystemNotification _sendSystemNotification = sendSystemNotification;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task ExecuteAsync(CallUserCompanionEvent @event, CancellationToken cancellationToken)
        {

            var adminId = await _userRepository.GetAdminId();
            string userFullName = await _userRepository.GetUserFullName(@event.UserId);
            var userPlanExcersiesAnswerInDb = await userPlanExcersiesAnswerRepository.Table.Where(t => t.Id == @event.Id)
                .Select(t => new
                {
                    Text = stepRepository.Table.Where(tt => tt.Id == t.StepId).Select(tt => tt.Name).FirstOrDefault(),
                    AnswerTitle = !string.IsNullOrEmpty(t.Text) ? t.Text : "چیزی ننوشته"
                })
                .FirstOrDefaultAsync(cancellationToken);
            var userCompanions = _userRepository.Table.Include(t => t.Companions).SelectMany(t => t.Companions).Where(a => a.UserPlanId == @event.UserPlanId).Select(t => t.CompanionUserId).ToList();
            if (adminId > 0)
            {
                if (userCompanions != null && userCompanions.Count > 0)
                    await _sendSystemNotification.SendNotification(
                        adminId,
                        UserStepAnswerMessages.NewUserStepAnswerSubject(userFullName),
                        UserStepAnswerMessages.NewUserStepAnswerDescription(userPlanExcersiesAnswerInDb.Text, userPlanExcersiesAnswerInDb.AnswerTitle),
                        false,
                        userCompanions,
                        true
                    );
            }
        }
    }
}
