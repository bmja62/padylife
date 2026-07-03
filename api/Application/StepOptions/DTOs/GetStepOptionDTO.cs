namespace Application.StepOptions.DTOs
{
    // DTO پایه
    public class GetStepOptionDTO
    {
        public long Id { get; set; }
        public long StepId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public string Type { get; set; }
    }

    // چند گزینه‌ای
    public class GetMultipleChoiceStepOptionDTO : GetStepOptionDTO
    {
        public List<OptionChoiceDTO> Choices { get; set; } = new();
        public bool AllowMultipleSelection { get; set; }
        public string CorrectAnswerHint { get; set; }
    }

    // ویدیو
    public class GetVideoStepOptionDTO : GetStepOptionDTO
    {
        public string VideoUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public TimeSpan Duration { get; set; }
        public bool AllowDownload { get; set; }
    }

    // تسک
    public class GetTaskStepOptionDTO : GetStepOptionDTO
    {
        public int DeadlineDays { get; set; }
        public string AssigneeRole { get; set; }
        public string TaskInstructions { get; set; }
        public int EstimatedMinutes { get; set; }
    }

    // عملیات
    public class GetActionStepOptionDTO : GetStepOptionDTO
    {
        public string ActionCommand { get; set; }
        public string ActionParameters { get; set; }
        public bool RequiresConfirmation { get; set; }
    }

    // متن
    public class GetTextStepOptionDTO : GetStepOptionDTO
    {
        public string Content { get; set; }
        public bool IsHtml { get; set; }
        public string TextFormat { get; set; }
    }

    // تصویر
    public class GetImageStepOptionDTO : GetStepOptionDTO
    {
        public string ImageUrl { get; set; }
        public string AltText { get; set; }
        public string Caption { get; set; }
    }
}
