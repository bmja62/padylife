using Entities.Medals;
using Services.Services.MedalServices;

public class MedalConditionFactory
{
    public IMedalCondition CreateCondition(string conditionType, string operatorStr, string value)
    {
        if (conditionType == "CompletedPlans" && operatorStr == ">=" && int.TryParse(value, out int minPlans))
        {
            return new MinimumPlanCompletedCondition(minPlans);
        }
        else if (conditionType == "MinimumAge" && operatorStr == ">=" && int.TryParse(value, out int minAge))
        {
            return new MinimumAgeCondition(minAge);
        }
        else if (conditionType == "MembershipDurationMonths" && operatorStr == ">=" && int.TryParse(value, out int minMembershipDurationMonths))
        {
            return new MembershipDurationMonthsCondition(minMembershipDurationMonths);
        }
        else if (conditionType == "TotalItemsOrdered" && operatorStr == ">=" && int.TryParse(value, out int minTotalItemsOrdered))
        {
            return new TotalItemsOrderedCondition(minTotalItemsOrdered);
        }

        throw new NotSupportedException($"Condition {conditionType} with operator {operatorStr} is not supported.");
    }

    public IMedalCondition CreateCompositeCondition(IEnumerable<MedalCondition> conditions)
    {
        var conditionObjects = conditions
            .Select(c => CreateCondition(c.ConditionType, c.Operator, c.Value))
            .ToList();

        return new AndCondition(conditionObjects); // یا OrCondition بسته به منطق شما
    }
}
