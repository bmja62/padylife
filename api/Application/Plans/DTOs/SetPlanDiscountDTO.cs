using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Plans.DTOs
{
    public class SetPlanDiscountDTO
    {
        public long PlanId { get; init; }
        public decimal? DiscountPrice { get; init; }
        public DateTime? DiscountPriceStartDate { get; init; }
        public DateTime? DiscountPriceEndDate { get; init; }
    }
}
