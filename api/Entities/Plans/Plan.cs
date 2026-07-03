using Entities.Common;
using Entities.Questions;
using Entities.Users;

namespace Entities.Plans
{
    public class Plan : BaseEntity<long>
    {
        private readonly List<ExpertPlanPrice> _planPrices = new();
        public Plan(string title, string imageUrl, long planCategoryId, bool isSignUpPlan, string description, string level, long ownerUserId, decimal? price)
        {
            Title = title;
            ImageUrl = imageUrl;
            PlanCategoryId = planCategoryId;
            IsSignUpPlan = isSignUpPlan;
            Description = description;
            OwnerUserId = ownerUserId;
            Level = level;
            Price = price;
        }

        private Plan() { }

        //Props
        public string Title { get; set; }
        public PlanStatus Status { get; private set; } = PlanStatus.Draft;
        public bool IsSignUpPlan { get; set; } = false;
        public string Description { get; set; }
        public string Level { get; set; }
        public decimal? Price { get; set; } = null;
        public string ImageUrl { get; set; }

        public decimal? DiscountPrice { get; set; } = null; // قیمت تخفیف
        public DateTime? DiscountPriceStartDate { get; set; } = null; // تاریخ شروع تخفیف
        public DateTime? DiscountPriceEndDate { get; set; } = null; // تاریخ پایان تخفیف

        // Property برای دریافت قیمت نهایی با در نظر گرفتن تخفیف
        public decimal? FinalPrice
        {
            get
            {
                if (Price == null) return null;

                // اگر تخفیف فعال باشد
                if (HasActiveDiscount())
                {
                    return Price - DiscountPrice;
                }

                return Price;
            }
        }


        //FKs
        public long PlanCategoryId { get; set; }
        public long OwnerUserId { get; set; }

        //Navigations
        public List<PlanQuestion> PlanQuestions { get; set; } = new();
        public List<UserPlan> UserPlans { get; set; } = new();
        public List<QuestionLinked> QuestionLinks { get; set; } = new();
        public List<PlanRelation> NextPlans { get; set; } = new();
        public List<PlanRelation> PreviousPlans { get; set; } = new();
        public PlanCategory PlanCategory { get; set; }
        public User OwnerUser { get; set; }

        public IReadOnlyCollection<ExpertPlanPrice> PlanPrices => _planPrices.AsReadOnly();
        //Methods
        public void Activate() => Status = PlanStatus.Active;

        public void AddPlanQuestions(List<PlanQuestion> planQuestions) => PlanQuestions.AddRange(planQuestions);



        public static Plan CreateDefault(
            string title,
            string imageUrl,
            long planCategoryId,
            bool isSignUpPlan,
            string description,
            string level,
            long ownerUserId,
            decimal? price
            ) => new(
                title,
                imageUrl,
                planCategoryId,
                isSignUpPlan,
                description,
                level,
                ownerUserId,
                price
                );

        public void AddPlanQuestion(PlanQuestion planQuestion) =>
            PlanQuestions.Add(planQuestion);

        public void AddQuestionLinkeds(List<QuestionLinked> questionLinks) =>
            QuestionLinks.AddRange(questionLinks);

        public void SetTitle(string title) => Title = title;

        public void SetPlanCategoryId(long planCategoryId) => PlanCategoryId = planCategoryId;

        public void SetStatus(PlanStatus status) => Status = status;

        public void SetIsSignUpPlan(bool isSignUpPlan) => IsSignUpPlan = isSignUpPlan;

        public void SetDescription(string description) => Description = description;
        public void SetLevel(string level) => Level = level;

        public void SetPrice(decimal? price) => Price = price;

        public void SetImageUrl(string imageUrl) => ImageUrl = imageUrl;

        // متدهای مدیریت تخفیف
        public void SetDiscount(decimal? discountPrice, DateTime? startDate, DateTime? endDate)
        {
            DiscountPrice = discountPrice;
            DiscountPriceStartDate = startDate;
            DiscountPriceEndDate = endDate;
        }

        public void RemoveDiscount()
        {
            DiscountPrice = null;
            DiscountPriceStartDate = null;
            DiscountPriceEndDate = null;
        }

        public bool HasActiveDiscount()
        {
            if (DiscountPrice == null || Price == null)
                return false;

            var now = DateTime.UtcNow;
            bool isInDateRange = (!DiscountPriceStartDate.HasValue || DiscountPriceStartDate <= now) &&
                               (!DiscountPriceEndDate.HasValue || DiscountPriceEndDate >= now);

            return isInDateRange && DiscountPrice < Price;
        }

        public bool IsDiscountValid()
        {
            return HasActiveDiscount();
        }




        // وضعیت فعلی پلن (اکتیو/تکمیل شده/...)
        public enum PlanStatus
        {
            Draft = 1,
            Active = 2
        }

    }
}
