using Entities.Common;

namespace Entities.Users
{
    public class UserPlanCompanion : BaseEntity<long>
    {
        public UserPlanCompanion(long userPlanId, long companionUserId)
        {
            UserPlanId = userPlanId;
            CompanionUserId = companionUserId;
            JoinedAt = DateTime.Now;
        }

        public long UserPlanId { get; private set; }
        public long CompanionUserId { get; private set; }
        public DateTime JoinedAt { get; private set; }

        public UserPlan UserPlan { get; private set; }
        public User CompanionUser { get; private set; }

        private UserPlanCompanion() { }

        public static UserPlanCompanion Join(long userPlanId, long companionUserId)
        {
            return new UserPlanCompanion
            {
                UserPlanId = userPlanId,
                CompanionUserId = companionUserId,
                JoinedAt = DateTime.UtcNow
            };
        }
    }


}
