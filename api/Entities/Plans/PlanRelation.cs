
using Entities.Common;

namespace Entities.Plans
{
    public class PlanRelation : IEntity
    {
        public long SourcePlanId { get; set; }
        public Plan SourcePlan { get; set; }

        public long TargetPlanId { get; set; }
        public Plan TargetPlan { get; set; }

        public int Order { get; set; }
    }

}
