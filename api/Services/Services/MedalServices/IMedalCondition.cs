using Entities.Users;
using System.Linq.Expressions;

namespace Services.Services.MedalServices
{
    public interface IMedalCondition
    {
        Expression<Func<User, bool>> ToExpression();
    }

    public class MinimumPlanCompletedCondition : IMedalCondition
    {
        private readonly int _minPlans;

        public MinimumPlanCompletedCondition(int minPlans)
        {
            _minPlans = minPlans;
        }

        public Expression<Func<User, bool>> ToExpression()
        {
            return user => user.UserPlans.Count(p => p.IsCompleted) >= _minPlans;
        }
    }


    public class MinimumAgeCondition : IMedalCondition
    {
        private readonly int _minAge;

        public MinimumAgeCondition(int minAge)
        {
            _minAge = minAge;
        }

        public Expression<Func<User, bool>> ToExpression()
        {
            return user => user.Age >= _minAge;
        }
    }

    public class MembershipDurationMonthsCondition : IMedalCondition
    {
        private readonly int _minMembershipDurationMonths;

        public MembershipDurationMonthsCondition(int minMembershipDurationMonths)
        {
            _minMembershipDurationMonths = minMembershipDurationMonths;
        }
        public Expression<Func<User, bool>> ToExpression()
        {
            return user => (int)(((DateTime.Now - user.CreateAt).TotalDays) / 30) >= _minMembershipDurationMonths;
        }
    }

    public class TotalItemsOrderedCondition : IMedalCondition
    {
        private readonly int _minTotalItemsOrderedCondition;

        public TotalItemsOrderedCondition(int minTotalItemsOrderedCondition)
        {
            _minTotalItemsOrderedCondition = minTotalItemsOrderedCondition;
        }

        public Expression<Func<User, bool>> ToExpression()
        {
            return user => user.Orders.Count >= _minTotalItemsOrderedCondition;
        }
    }

    public class AndCondition : IMedalCondition
    {
        private readonly IEnumerable<IMedalCondition> _conditions;

        public AndCondition(IEnumerable<IMedalCondition> conditions)
        {
            _conditions = conditions;
        }

        public Expression<Func<User, bool>> ToExpression()
        {
            var conditions = _conditions.ToList();
            if (!conditions.Any())
                return user => true;

            var expr = conditions[0].ToExpression();
            foreach (var cond in conditions.Skip(1))
            {
                expr = expr.AndAlso(cond.ToExpression());
            }

            return expr;
        }
    }


    public class OrCondition : IMedalCondition
    {
        private readonly IEnumerable<IMedalCondition> _conditions;

        public OrCondition(IEnumerable<IMedalCondition> conditions)
        {
            _conditions = conditions;
        }

        public Expression<Func<User, bool>> ToExpression()
        {
            var conditions = _conditions.ToList();
            if (!conditions.Any())
                return user => false;

            var expr = conditions[0].ToExpression();
            foreach (var cond in conditions.Skip(1))
            {
                expr = expr.OrElse(cond.ToExpression());
            }

            return expr;
        }
    }


}
