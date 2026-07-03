using Entities.Excersies;

namespace Entities.StepOprions
{
    public static class StepOptionFactory
    {
        public static VideoStepOption CreateVideo(long stepId, string title, string videoUrl, TimeSpan duration, string thumbnailUrl, bool allowDownload) =>
            new VideoStepOption
            {
                StepId = stepId,
                Title = title,
                VideoUrl = videoUrl,
                Duration = duration,
                ThumbnailUrl = thumbnailUrl,
                AllowDownload = allowDownload,
                Type = StepOptionType.Video
            };

        public static TaskStepOption CreateTask(long stepId, string title, string assigneeRole, int estimatedMinutes, string taskInstructions, int deadline) =>
            new TaskStepOption
            {
                StepId = stepId,
                Title = title,
                AssigneeRole = assigneeRole,
                EstimatedMinutes = estimatedMinutes,
                TaskInstructions = taskInstructions,
                DeadlineDays = deadline,
                Type = StepOptionType.Task

            };

        public static MultipleChoiceStepOption CreateMultipleChoice(long stepId, string title, List<OptionChoice> choices, bool allowMultipleSelection, string correctAnswerHint = "") =>
            new MultipleChoiceStepOption
            {
                StepId = stepId,
                Title = title,
                Choices = choices,
                AllowMultipleSelection = allowMultipleSelection,
                CorrectAnswerHint = correctAnswerHint,
                Type = StepOptionType.MultipleChoice
            };

        public static ActionStepOption CreateAction(long stepId, string title, string actionCommand, string actionParameters, bool requiresConfirmation) =>
            new ActionStepOption
            {
                StepId = stepId,
                Title = title,
                ActionCommand = actionCommand,
                ActionParameters = actionParameters,
                RequiresConfirmation = requiresConfirmation,
                Type = StepOptionType.Action
            };

        public static TextStepOption CreateText(long stepId, string title, string content, bool isHtml, string textFormat) =>
            new TextStepOption
            {
                StepId = stepId,
                Title = title,
                Content = content,
                IsHtml = isHtml,
                TextFormat = textFormat,
                Type = StepOptionType.Text
            };

        public static ImageStepOption CreateImage(long stepId, string title, string imageUrl, string altText, string caption) =>
            new ImageStepOption
            {
                StepId = stepId,
                Title = title,
                ImageUrl = imageUrl,
                AltText = altText,
                Caption = caption,
                Type = StepOptionType.Image
            };


    }

}
