namespace Entities.StepOprions
{
    public static class StepOptionFluentExtensions
    {
        public static T WithDescription<T>(this T option, string description) where T : StepOption
        {
            option.Description = description;
            return option;
        }

        public static T WithOrder<T>(this T option, int order) where T : StepOption
        {
            option.Order = order;
            return option;
        }
    }

}
