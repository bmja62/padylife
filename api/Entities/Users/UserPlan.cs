using Entities.Chats;
using Entities.Common;
using Entities.Plans;

namespace Entities.Users
{
    // اجرای پلن توسط کاربر
    public class UserPlan : BaseEntity<long>
    {
        private readonly List<UserPlanCompanion> _companions = new();
        private readonly List<UserPlanExpert> _experts = new();
        private readonly List<ChatRoom> _chats = new();
        public UserPlan(long planId, long userId, bool isSignUpPlan)
        {
            PlanId = planId;
            UserId = userId;
            IsSignUpPlan = isSignUpPlan;
        }

        private UserPlan() { }
        public long UserId { get; set; }
        public long PlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsSignUpPlan { get; set; } = false;
        public bool IsCompleted { get; set; } = false;

        // Navigations
        public User User { get; set; }
        public Plan Plan { get; set; }
        public List<UserPlanAnswer> Answers { get; set; }

        public IReadOnlyCollection<UserPlanCompanion> Companions => _companions.AsReadOnly();
        public IReadOnlyCollection<UserPlanExpert> Experts => _experts.AsReadOnly();
        public IReadOnlyCollection<ChatRoom> Chats => _chats.AsReadOnly();

        public static UserPlan CreateDefault(long planId, long userId, bool isSignUpPlan) => new(planId, userId, isSignUpPlan);

        public void SetEndDate(DateTime endDate) => EndDate = endDate;

        public void AddExpert(UserPlanExpert expert) => _experts.Add(expert);

        public void AddCompanion(UserPlanCompanion companion) => _companions.Add(companion);

        public void AddChat(ChatRoom chat) => _chats.Add(chat);

        public void SetIsCompleted()
        {
            EndDate = DateTime.Now;
            IsCompleted = true;
        }
    }
}
