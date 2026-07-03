using Microsoft.Extensions.Logging;
using Services.Services.Visits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.JobsServices
{
    public class CleanupOldVisitDataJob
    {
        private readonly IVisitTrackingService _visitTrackingService;
        private readonly ILogger<CleanupOldVisitDataJob> _logger;

        public CleanupOldVisitDataJob(
            IVisitTrackingService visitTrackingService,
            ILogger<CleanupOldVisitDataJob> logger)
        {
            _visitTrackingService = visitTrackingService;
            _logger = logger;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("CleanupOldVisitDataJob started at {Time}", DateTime.UtcNow);

                // پاک‌سازی داده‌های قدیمی‌تر از 365 روز
                var result = await _visitTrackingService.CleanupOldDataAsync(365);

                if (result.IsSuccess)
                {
                    _logger.LogInformation("CleanupOldVisitDataJob completed successfully at {Time}", DateTime.UtcNow);
                }
                else
                {
                    _logger.LogWarning("CleanupOldVisitDataJob completed with warnings: {Message} at {Time}",
                        result.Message, DateTime.UtcNow);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در پاک‌سازی داده‌های قدیمی در CleanupOldVisitDataJob");
                throw;
            }
        }
    }
}
