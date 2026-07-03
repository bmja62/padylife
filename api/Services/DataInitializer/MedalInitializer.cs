using Data.Contracts;
using Entities.Medals;

namespace Services.DataInitializer
{
    public class MedalInitializer(
        IRepository<Medal> medalRepo,
        IRepository<MedalCondition> conditionRepo) : IDataInitializer
    {
        public void InitializeData()
        {
            if (medalRepo.Table.Any())
                return;

            var medals = new List<(string Title, string Description, string IconUrl, List<(string ConditionType, string Operator, string Value)> Conditions)>
            {
                (
                    "شروع‌کننده",
                    "برای تکمیل اولین برنامه غذایی اهدا می‌شود",
                    "/icons/starter.png",
                    new List<(string, string, string)>
                    {
                        ("CompletedPlans", ">=", "1")
                    }
                ),
                (
                    "کاربر منظم",
                    "برای تکمیل ۵ برنامه غذایی اهدا می‌شود",
                    "/icons/regular.png",
                    new List<(string, string, string)>
                    {
                        ("CompletedPlans", ">=", "5")
                    }
                ),
                (
                    "عضو وفادار",
                    "برای عضویت بیش از ۶ ماه اهدا می‌شود",
                    "/icons/loyal.png",
                    new List<(string, string, string)>
                    {
                        ("MembershipDurationMonths", ">=", "6")
                    }
                ),
                (
                    "خریدار عمده",
                    "برای سفارش بیش از ۵۰ مورد اهدا می‌شود",
                    "/icons/bulk.png",
                    new List<(string, string, string)>
                    {
                        ("TotalItemsOrdered", ">", "50")
                    }
                )
            };

            foreach (var (title, description, iconUrl, conditions) in medals)
            {
                var medal = new Medal(title, description, iconUrl);
                medalRepo.Add(medal);

                foreach (var (conditionType, op, value) in conditions)
                {
                    var condition = new MedalCondition(medal.Id, conditionType, op, value);
                    conditionRepo.Add(condition);
                }
            }
        }
    }
}
