using Application.Cqrs.Commands;
using FluentValidation;
using Services;
using Services.Services.Visits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Visits.Commands
{
    public class ProcessDailyStatsCommand : ICommand<ServiceResult>
    {
        public DateTime TargetDate { get; }

        public ProcessDailyStatsCommand(DateTime targetDate)
        {
            TargetDate = targetDate.Date; // فقط تاریخ بدون زمان
        }
    }

    public class ProcessDailyStatsCommandValidator : AbstractValidator<ProcessDailyStatsCommand>
    {
        public ProcessDailyStatsCommandValidator()
        {
            RuleFor(x => x.TargetDate)
                .LessThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage("تاریخ هدف نمی‌تواند در آینده باشد")
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date.AddYears(-1))
                .WithMessage("تاریخ هدف نمی‌تواند بیش از یک سال گذشته باشد");
        }
    }

    public class ProcessDailyStatsCommandHandler : ICommandHandler<ProcessDailyStatsCommand, ServiceResult>
    {
        private readonly IVisitTrackingService _visitTrackingService;

        public ProcessDailyStatsCommandHandler(IVisitTrackingService visitTrackingService)
        {
            _visitTrackingService = visitTrackingService;
        }

        public async Task<ServiceResult> Handle(ProcessDailyStatsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _visitTrackingService.ProcessDailyStatsAsync(request.TargetDate);
                return ServiceResult.Ok("آمار روزانه با موفقیت پردازش شد");
            }
            catch (Exception ex)
            {
                return ServiceResult.Fail($"خطا در پردازش آمار روزانه: {ex.Message}");
            }
        }
    }
}
