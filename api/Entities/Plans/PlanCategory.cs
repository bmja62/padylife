using Entities.Common;

namespace Entities.Plans
{
    public class PlanCategory : BaseEntity<long>
    {
        public PlanCategory() { }
        public string Name { get; set; }

        //Navigation
        public List<Plan> Plans { get; set; } = new();
    }
}
