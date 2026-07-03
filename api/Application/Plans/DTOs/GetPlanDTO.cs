using Entities.Plans;

namespace Application.Plans.DTOs
{
    public class GetPlanDTO
    {
        public GetPlanDTO(long id)
        {
            Id = id;
        }

        public GetPlanDTO(
            long id,
            string title,
            string imageUrl,
            long planCategoryId,
            string planCategoryName,
            string description,
            bool isSignUpPlan,
            Entities.Plans.Plan.PlanStatus status,
            string level,
            decimal? price,
            decimal? discountPrice,
            decimal? finalPrice,
            List<PlanQuestionDTO> planQuestions
            )
        {
            Id = id;
            Title = title;
            ImageUrl = imageUrl;
            PlanCategoryId = planCategoryId;
            PlanCategoryName = planCategoryName;
            Description = description;
            IsSignUpPlan = isSignUpPlan;
            PlanQuestions = planQuestions;
            Status = status;
            Level = level;
            Price = price;
            DiscountPrice = discountPrice;
            FinalPrice = finalPrice;    
        }

        public long Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public long PlanCategoryId { get; set; }
        public string PlanCategoryName { get; set; }
        public string Description { get; internal set; }
        public bool IsSignUpPlan { get; internal set; }
        public List<PlanQuestionDTO> PlanQuestions { get; set; }
        public Plan.PlanStatus Status { get; private set; }
        public string Level { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal? FinalPrice { get; set; }

        internal static GetPlanDTO CreateDefault(
            long id,
            string title,
            string imageUrl,
            long planCategoryId,
            string planCategoryName,
            string description,
            bool isSignUpPlan,
            Entities.Plans.Plan.PlanStatus status,
            string level,
            decimal? price,
            decimal? discountPrice,
            decimal? finalPrice,
            List<PlanQuestionDTO> planQuestions)
        => new(
            id,
            title,
            imageUrl,
            planCategoryId,
            planCategoryName,
            description,
            isSignUpPlan,
            status,
            level,
            price,
            discountPrice,
            finalPrice,
            planQuestions
            );

        internal static GetPlanDTO GetPlanId(long id)
        => new(id);
    }
}
