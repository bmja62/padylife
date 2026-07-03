using Common.GridResults;

namespace Application.StepOptions.DTOs
{
    public abstract class BaseStepOptionDTO
    {
        public long StepId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public long? CreatedByUserId { get; set; }
    }
    public class CreateVideoStepOptionCommandDTO : BaseStepOptionDTO
    {
        public string VideoUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public bool AllowDownload { get; set; }
    }
    public class CreateMultipleChoiceStepOptionCommandDTO : BaseStepOptionDTO
    {
        public List<OptionChoiceDTO> Choices { get; set; }
        public bool AllowMultipleSelection { get; set; }
        public string CorrectAnswerHint { get; set; }
    }
    public class OptionChoiceDTO
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public int Order { get; set; }
    }
    public class CreateTextStepOptionCommandDTO : BaseStepOptionDTO
    {
        public string Content { get; set; }
        public bool IsHtml { get; set; }
        public string TextFormat { get; set; }
    }
    public class CreateTaskStepOptionCommandDTO : BaseStepOptionDTO
    {
        public int DeadlineDays { get; set; }
        public string AssigneeRole { get; set; }
        public string TaskInstructions { get; set; }
        public int EstimatedMinutes { get; set; }
    }
    public class CreateProcessStepOptionCommandDTO : BaseStepOptionDTO
    {
        public string ProcessDefinitionId { get; set; }
        public string RequiredRole { get; set; }
        public string ProcessDiagramUrl { get; set; }
    }
    public class CreateActionStepOptionCommandDTO : BaseStepOptionDTO
    {
        public string ActionCommand { get; set; }
        public string ActionParameters { get; set; }
        public bool RequiresConfirmation { get; set; }
    }
    public class CreateImageStepOptionCommandDTO : BaseStepOptionDTO
    {
        public string ImageUrl { get; set; }
        public string AltText { get; set; }
        public string Caption { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
    }
    public class GetAllStepOptionsQueryDTO : GlobalGrid
    {
        public long? StepId { get; set; }
        public string Type { get; set; }
    }






}
