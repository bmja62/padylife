using Entities.Common;
using Entities.Plans;

namespace Entities.Users
{
    public class ExpertPlanPrice : BaseEntity<long>
    {
        public ExpertPlanPrice(long expertId, long planId, decimal price)
        {
            ExpertId = expertId;
            PlanId = planId;
            Price = price;
            IsActive = true;
        }

        public long ExpertId { get; private set; }
        public long PlanId { get; private set; }
        public decimal Price { get; private set; }
        public bool IsActive { get; private set; }

        public Expert Expert { get; set; }
        public Plan Plan { get; set; }
        private ExpertPlanPrice() { }

        public static ExpertPlanPrice Create(long expertId, long planId, decimal price)
        {
            return new ExpertPlanPrice
            {
                ExpertId = expertId,
                PlanId = planId,
                Price = price,
                IsActive = true
            };
        }

        public void Deactivate() => IsActive = !IsActive;

        public void UpdatePrice(decimal newPrice) => Price = newPrice;
    }
}
