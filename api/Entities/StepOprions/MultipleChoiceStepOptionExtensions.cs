using Entities.Excersies;

namespace Entities.StepOprions
{
    public static class MultipleChoiceStepOptionExtensions
    {
        public static MultipleChoiceStepOption AddChoice(this MultipleChoiceStepOption option, OptionChoice choice)
        {
            option.Choices.Add(choice);
            return option;
        }

        public static MultipleChoiceStepOption WithCorrectHint(this MultipleChoiceStepOption option, string hint)
        {
            option.CorrectAnswerHint = hint;
            return option;
        }
    }

}
