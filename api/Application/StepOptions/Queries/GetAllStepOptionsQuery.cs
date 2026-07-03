using Application.Cqrs.Queris;
using Application.StepOptions.DTOs;
using Common.GridResults;
using Data.Contracts;
using Data.Repositories.Extentions;
using Entities.Excersies;
using Entities.StepOprions;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Application.StepOptions.Queries
{
    public class GetAllStepOptionsQuery : IQuery<ServiceResult<GlobalGridResult<GetStepOptionDTO>>>
    {
        public GetAllStepOptionsQuery(GetAllStepOptionsQueryDTO input)
        {
            Input = input;
        }

        public GetAllStepOptionsQueryDTO Input { get; }
    }
    public class GetAllStepOptionsQueryHandler : IQueryHandler<GetAllStepOptionsQuery, ServiceResult<GlobalGridResult<GetStepOptionDTO>>>
    {
        private readonly IRepository<StepOption> _stepOptionRepository;
        private readonly IRepository<OptionChoice> _optionChoiceRepository;  // اضافه کردن Repository برای OptionChoice

        public GetAllStepOptionsQueryHandler(IRepository<StepOption> stepOptionRepository, IRepository<OptionChoice> optionChoiceRepository)
        {
            _stepOptionRepository = stepOptionRepository;
            _optionChoiceRepository = optionChoiceRepository;
        }

        public async Task<ServiceResult<GlobalGridResult<GetStepOptionDTO>>> Handle(GetAllStepOptionsQuery request, CancellationToken cancellationToken)
        {
            var query = _stepOptionRepository.TableNoTracking
        .Include(so => so.Step) // اگر نیاز به اطلاعات Step دارید
        .AsQueryable();

            // فیلترها
            if (request.Input.StepId.HasValue)
                query = query.Where(t => t.StepId == request.Input.StepId.Value);

            if (!string.IsNullOrEmpty(request.Input.Type))
                query = query.Where(t => t.Type.ToString() == request.Input.Type);

            var totalCount = await query.CountAsync(cancellationToken);

            // تمام موجودیت‌ها را با هم بارگیری می‌کنیم
            var stepOptions = await query
                .ApplyPagination(request.Input.PageNumber ?? 1, request.Input.Count ?? 20)
                .OrderBy(t => t.Order)
                .ToListAsync(cancellationToken);

            // تبدیل به DTOهای مناسب
            var data = stepOptions.Select(ConvertToDto).ToList();

            return ServiceResult.Ok(new GlobalGridResult<GetStepOptionDTO>
            {
                Data = data,
                TotalCount = totalCount
            });
        }

        private GetStepOptionDTO ConvertToDto(StepOption stepOption)
        {
            switch (stepOption)
            {
                case MultipleChoiceStepOption mc:
                    return new GetMultipleChoiceStepOptionDTO
                    {
                        Id = mc.Id,
                        StepId = mc.StepId,
                        Title = mc.Title,
                        Description = mc.Description,
                        Order = mc.Order,
                        Type = mc.Type.ToString(),
                        Choices = _optionChoiceRepository.TableNoTracking
                            .Where(oc => oc.StepOptionId == mc.Id)
                            .OrderBy(oc => oc.Order)
                            .Select(oc => new OptionChoiceDTO
                            {
                                Id = oc.Id,
                                Text = oc.Text,
                                IsCorrect = oc.IsCorrect,
                                Order = oc.Order
                            })
                            .ToList(),
                        AllowMultipleSelection = mc.AllowMultipleSelection,
                        CorrectAnswerHint = mc.CorrectAnswerHint
                    };

                case VideoStepOption video:
                    return new GetVideoStepOptionDTO
                    {
                        Id = video.Id,
                        StepId = video.StepId,
                        Title = video.Title,
                        Description = video.Description,
                        Order = video.Order,
                        Type = video.Type.ToString(),
                        VideoUrl = video.VideoUrl,
                        ThumbnailUrl = video.ThumbnailUrl,
                        Duration = video.Duration,
                        AllowDownload = video.AllowDownload
                    };

                case TaskStepOption task:
                    return new GetTaskStepOptionDTO
                    {
                        Id = task.Id,
                        StepId = task.StepId,
                        Title = task.Title,
                        Description = task.Description,
                        Order = task.Order,
                        Type = task.Type.ToString(),
                        DeadlineDays = task.DeadlineDays,
                        AssigneeRole = task.AssigneeRole,
                        TaskInstructions = task.TaskInstructions,
                        EstimatedMinutes = task.EstimatedMinutes
                    };

                case ActionStepOption action:
                    return new GetActionStepOptionDTO
                    {
                        Id = action.Id,
                        StepId = action.StepId,
                        Title = action.Title,
                        Description = action.Description,
                        Order = action.Order,
                        Type = action.Type.ToString(),
                        ActionCommand = action.ActionCommand,
                        ActionParameters = action.ActionParameters,
                        RequiresConfirmation = action.RequiresConfirmation
                    };

                case TextStepOption text:
                    return new GetTextStepOptionDTO
                    {
                        Id = text.Id,
                        StepId = text.StepId,
                        Title = text.Title,
                        Description = text.Description,
                        Order = text.Order,
                        Type = text.Type.ToString(),
                        Content = text.Content,
                        IsHtml = text.IsHtml,
                        TextFormat = text.TextFormat
                    };

                case ImageStepOption image:
                    return new GetImageStepOptionDTO
                    {
                        Id = image.Id,
                        StepId = image.StepId,
                        Title = image.Title,
                        Description = image.Description,
                        Order = image.Order,
                        Type = image.Type.ToString(),
                        ImageUrl = image.ImageUrl,
                        AltText = image.AltText,
                        Caption = image.Caption
                    };

                default:
                    return new GetStepOptionDTO
                    {
                        Id = stepOption.Id,
                        StepId = stepOption.StepId,
                        Title = stepOption.Title,
                        Description = stepOption.Description,
                        Order = stepOption.Order,
                        Type = stepOption.Type.ToString()
                    };
            }
        }
    }


}
