using Microsoft.Extensions.Logging;
using Services.Services.Visits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.JobsServices
{
    public class ProcessDailyStatsJob
    {
        private readonly IVisitTrackingService _visitTrackingService;
        private readonly ILogger<ProcessDailyStatsJob> _logger;

        public ProcessDailyStatsJob(
            IVisitTrackingService visitTrackingService,
            ILogger<ProcessDailyStatsJob> logger)
        {
            _visitTrackingService = visitTrackingService;
            _logger = logger;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("ProcessDailyStatsJob started at {Time}", DateTime.UtcNow);

                // پردازش آمار روز قبل
                var targetDate = DateTime.UtcNow.Date.AddDays(-1);

                var result = await _visitTrackingService.ProcessDailyStatsAsync(targetDate);

                if (result.IsSuccess)
                {
                    _logger.LogInformation("ProcessDailyStatsJob completed successfully for date {TargetDate} at {Time}",
                        targetDate.ToString("yyyy-MM-dd"), DateTime.UtcNow);
                }
                else
                {
                    _logger.LogWarning("ProcessDailyStatsJob completed with warnings for date {TargetDate}: {Message} at {Time}",
                        targetDate.ToString("yyyy-MM-dd"), result.Message, DateTime.UtcNow);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در پردازش آمار روزانه در ProcessDailyStatsJob");
                throw;
            }
        }
    }
    public class ProcessPer10MINStatsJob
    {
        private readonly IVisitTrackingService _visitTrackingService;
        private readonly ILogger<ProcessDailyStatsJob> _logger;

        public ProcessPer10MINStatsJob(
            IVisitTrackingService visitTrackingService,
            ILogger<ProcessDailyStatsJob> logger)
        {
            _visitTrackingService = visitTrackingService;
            _logger = logger;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("ProcessDailyStatsJob started at {Time}", DateTime.UtcNow);

                // پردازش  روز
                var targetDate = DateTime.UtcNow.Date;

                var result = await _visitTrackingService.ProcessDailyStatsAsync(targetDate);

                if (result.IsSuccess)
                {
                    _logger.LogInformation("ProcessDailyStatsJob completed successfully for date {TargetDate} at {Time}",
                        targetDate.ToString("yyyy-MM-dd"), DateTime.UtcNow);
                }
                else
                {
                    _logger.LogWarning("ProcessDailyStatsJob completed with warnings for date {TargetDate}: {Message} at {Time}",
                        targetDate.ToString("yyyy-MM-dd"), result.Message, DateTime.UtcNow);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در پردازش آمار روزانه در ProcessDailyStatsJob");
                throw;
            }
        }
    }
}
