namespace Entities.Users
{
    public class Expert : User
    {
        private readonly List<UserPlanExpert> _userPlanExperts = new();
        private readonly List<ExpertPlanPrice> _planPrices = new();

        public Expert(string userName, string phoneNumber, string email) : base(userName, phoneNumber, email)
        {
        }

        public IReadOnlyCollection<UserPlanExpert> UserPlanExperts => _userPlanExperts.AsReadOnly();
        public IReadOnlyCollection<ExpertPlanPrice> PlanPrices => _planPrices.AsReadOnly();

        public static Expert RegisterExpert(string userName, string phoneNumber, string email) =>
        new(userName, phoneNumber, email);

        public void AddUserPlanExpert(UserPlanExpert expert) => _userPlanExperts.Add(expert);

        public void SetPlanPrice(ExpertPlanPrice price) => _planPrices.Add(price);
    }
}
