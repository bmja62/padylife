using Entities.Common;

namespace Entities.Users
{
    public class UserPlanExpert : BaseEntity<long>
    {
        private UserPlanExpert()
        {

        }
        public UserPlanExpert(long userPlanId, long expertId, decimal price, string specialization)
        {
            UserPlanId = userPlanId;
            ExpertId = expertId;
            Price = price;
            Specialization = specialization;
            JoinedAt = DateTime.Now;
        }

        public long UserPlanId { get; private set; }
        public long ExpertId { get; private set; }
        public decimal Price { get; private set; }
        public string Specialization { get; private set; } // تخصص خاص (مثلا تغذیه، بدنسازی)
        public DateTime JoinedAt { get; private set; }

        // روابط
        public UserPlan UserPlan { get; set; }
        public Expert Expert { get; set; }


        public static UserPlanExpert Assign(long userPlanId, long expertId, decimal price, string specialization) => new UserPlanExpert
        {
            UserPlanId = userPlanId,
            ExpertId = expertId,
            Price = price,
            Specialization = specialization,
            JoinedAt = DateTime.UtcNow
        };
        // متدها
        public void UpdatePrice(decimal newPrice) => Price = newPrice;
        public void UpdateSpecialization(string newSpecialization) => Specialization = newSpecialization;
    }
}
