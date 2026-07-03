using Entities.Common;
using Entities.Excersies;
using Entities.Users;

namespace Entities.StepOprions
{
    // کلاس پایه
    public abstract class StepOption : BaseEntity<long>
    {
        public long StepId { get; set; }
        public StepOptionType Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public long? CreatedByUserId { get; set; }

        public User CreatedByUser { get; set; }
        public Step Step { get; set; }

        public ICollection<UserPlanExcersiesAnswer> UserPlanExcersiesAnswers { get; set; }

    }

    // چند گزینه‌ای
    public class MultipleChoiceStepOption : StepOption
    {
        public List<OptionChoice> Choices { get; set; } = new();
        public bool AllowMultipleSelection { get; set; }
        public string CorrectAnswerHint { get; set; }

    }

    // ویدیو
    public class VideoStepOption : StepOption
    {
        public string VideoUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public bool AllowDownload { get; set; }
    }
    // تسک
    public class TaskStepOption : StepOption
    {
        public int DeadlineDays { get; set; }
        public string AssigneeRole { get; set; }
        public string TaskInstructions { get; set; }
        public int EstimatedMinutes { get; set; }
    }

    // عملیات
    public class ActionStepOption : StepOption
    {
        public string ActionCommand { get; set; }
        public string ActionParameters { get; set; }
        public bool RequiresConfirmation { get; set; }
    }

    // متن
    public class TextStepOption : StepOption
    {
        public string Content { get; set; }
        public bool IsHtml { get; set; }
        public string TextFormat { get; set; } // Markdown, PlainText, etc.
    }

    // تصویر
    public class ImageStepOption : StepOption
    {
        public string ImageUrl { get; set; }
        public string AltText { get; set; }
        public string Caption { get; set; }
    }

}
