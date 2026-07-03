using Common;
using Data.Contracts;
using Entities.Visits;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Services.Visits.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.Visits
{
    public interface IVisitTrackingService : IScopedDependency
    {
        /// <summary>
        /// پردازش آمار روزانه برای تاریخ مشخص
        /// </summary>
        Task<ServiceResult> ProcessDailyStatsAsync(DateTime date);

        /// <summary>
        /// پردازش آمار برای بازه زمانی مشخص
        /// </summary>
        Task<ServiceResult> ProcessDateRangeStatsAsync(DateTime fromDate, DateTime toDate);

        /// <summary>
        /// دریافت آمار بازدید یک موجودیت خاص
        /// </summary>
        Task<EntityVisitStats> GetEntityStatsAsync(string entityType, long? entityId = null, DateTime? fromDate = null, DateTime? toDate = null);

        /// <summary>
        /// دریافت آمار بازدید یک صفحه خاص
        /// </summary>
        Task<PageVisitStats> GetPageStatsAsync(string pageUrl, DateTime? fromDate = null, DateTime? toDate = null);

        /// <summary>
        /// دریافت پر بازدیدترین صفحات
        /// </summary>
        Task<List<PopularPage>> GetPopularPagesAsync(int top = 10, DateTime? fromDate = null, DateTime? toDate = null);

        /// <summary>
        /// دریافت آمار بازدید بر اساس بخش‌ها
        /// </summary>
        Task<List<SectionStats>> GetSectionStatsAsync(DateTime? fromDate = null, DateTime? toDate = null);

        /// <summary>
        /// دریافت آمار بازدید روزانه برای نمودار
        /// </summary>
        Task<List<DailyChartData>> GetDailyChartDataAsync(string entityType = null, long? entityId = null, int days = 30);

        /// <summary>
        /// پاک‌سازی داده‌های قدیمی
        /// </summary>
        Task<ServiceResult> CleanupOldDataAsync(int keepDays = 365);

        /// <summary>
        /// دریافت خلاصه آمار
        /// </summary>
        Task<VisitSummary> GetVisitSummaryAsync(DateTime? fromDate = null, DateTime? toDate = null);
    }

    public class VisitTrackingService : IVisitTrackingService
    {
        private readonly IRepository<PageVisit> _pageVisitRepository;
        private readonly IRepository<DailyVisitStats> _dailyStatsRepository;
        private readonly ILogger<VisitTrackingService> _logger;

        public VisitTrackingService(
            IRepository<PageVisit> pageVisitRepository,
            IRepository<DailyVisitStats> dailyStatsRepository,
            ILogger<VisitTrackingService> logger)
        {
            _pageVisitRepository = pageVisitRepository;
            _dailyStatsRepository = dailyStatsRepository;
            _logger = logger;
        }

        public async Task<ServiceResult> ProcessDailyStatsAsync(DateTime date)
        {
            try
            {
                var targetDate = date.Date;
                var nextDate = targetDate.AddDays(1);

                _logger.LogInformation("Start processing daily stats for {TargetDate}", targetDate.ToString("yyyy-MM-dd"));

                // حذف آمار قبلی برای همان تاریخ
                var existingStats = await _dailyStatsRepository.Table
                    .Where(s => s.StatDate == targetDate)
                    .ToListAsync();

                if (existingStats.Any())
                {
                    _dailyStatsRepository.DeleteRange(existingStats);
                    
                    _logger.LogInformation("Deleted {Count} existing stats for {TargetDate}", existingStats.Count, targetDate.ToString("yyyy-MM-dd"));
                }

                // محاسبه آمار جدید از داده‌های خام
                var dailyStats = await _pageVisitRepository.TableNoTracking
                    .Where(v => v.VisitDate >= targetDate && v.VisitDate < nextDate)
                    .GroupBy(v => new { v.PageUrl, v.DetectedEntityType, v.DetectedEntityId, v.DetectedSection })
                    .Select(g => new DailyVisitStats
                    {
                        StatDate = targetDate,
                        PageUrl = g.Key.PageUrl,
                        DetectedEntityType = g.Key.DetectedEntityType,
                        DetectedEntityId = g.Key.DetectedEntityId,
                        DetectedSection = g.Key.DetectedSection,
                        UniqueVisits = g.Count(v => v.IsUniqueVisit),
                        TotalVisits = g.Count()
                    })
                    .ToListAsync();

                await _dailyStatsRepository.AddRangeAsync(dailyStats,CancellationToken.None);
      

                _logger.LogInformation("Successfully processed {Count} daily stats for {TargetDate}", dailyStats.Count, targetDate.ToString("yyyy-MM-dd"));

                return ServiceResult.Ok($"آمار روزانه برای {targetDate:yyyy-MM-dd} با موفقیت پردازش شد");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing daily stats for {TargetDate}", date.ToString("yyyy-MM-dd"));
                return ServiceResult.Fail($"خطا در پردازش آمار روزانه: {ex.Message}");
            }
        }

        public async Task<ServiceResult> ProcessDateRangeStatsAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var currentDate = fromDate.Date;
                var results = new List<string>();

                while (currentDate <= toDate.Date)
                {
                    var result = await ProcessDailyStatsAsync(currentDate);
                    if (result.IsSuccess)
                    {
                        results.Add($"✅ {currentDate:yyyy-MM-dd}: موفق");
                    }
                    else
                    {
                        results.Add($"❌ {currentDate:yyyy-MM-dd}: {result.Message}");
                    }
                    currentDate = currentDate.AddDays(1);
                }

                var message = string.Join("\n", results);
                return ServiceResult.Ok(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing date range stats from {FromDate} to {ToDate}", fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"));
                return ServiceResult.Fail($"خطا در پردازش آمار بازه زمانی: {ex.Message}");
            }
        }

        public async Task<EntityVisitStats> GetEntityStatsAsync(string entityType, long? entityId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _dailyStatsRepository.TableNoTracking
                .Where(s => s.DetectedEntityType == entityType);

            if (entityId.HasValue)
                query = query.Where(s => s.DetectedEntityId == entityId);

            if (fromDate.HasValue)
                query = query.Where(s => s.StatDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(s => s.StatDate <= toDate.Value);

            var stats = await query
                .GroupBy(s => 1)
                .Select(g => new EntityVisitStats
                {
                    EntityType = entityType,
                    EntityId = entityId,
                    TotalVisits = g.Sum(s => s.TotalVisits),
                    UniqueVisits = g.Sum(s => s.UniqueVisits)
                })
                .FirstOrDefaultAsync();

            if (stats == null)
            {
                return new EntityVisitStats
                {
                    EntityType = entityType,
                    EntityId = entityId,
                    TotalVisits = 0,
                    UniqueVisits = 0
                };
            }

            // محاسبه نرخ رشد
            stats.GrowthRate = await CalculateGrowthRateAsync(entityType, entityId, fromDate, toDate);

            // داده‌های روزانه برای نمودار
            stats.DailyData = await GetDailyVisitDataAsync(entityType, entityId, fromDate, toDate);

            return stats;
        }

        public async Task<PageVisitStats> GetPageStatsAsync(string pageUrl, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _dailyStatsRepository.TableNoTracking
                .Where(s => s.PageUrl == pageUrl);

            if (fromDate.HasValue)
                query = query.Where(s => s.StatDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(s => s.StatDate <= toDate.Value);

            var stats = await query
                .GroupBy(s => s.PageUrl)
                .Select(g => new PageVisitStats
                {
                    PageUrl = g.Key,
                    TotalVisits = g.Sum(s => s.TotalVisits),
                    UniqueVisits = g.Sum(s => s.UniqueVisits)
                })
                .FirstOrDefaultAsync();

            if (stats == null)
            {
                return new PageVisitStats
                {
                    PageUrl = pageUrl,
                    TotalVisits = 0,
                    UniqueVisits = 0
                };
            }

            // محاسبه منابع بازدید
            stats.VisitSources = await GetVisitSourcesAsync(pageUrl, fromDate, toDate);

            return stats;
        }

        public async Task<List<PopularPage>> GetPopularPagesAsync(int top = 10, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _dailyStatsRepository.TableNoTracking.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(s => s.StatDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(s => s.StatDate <= toDate.Value);

            var popularPages = await query
                .GroupBy(s => new { s.PageUrl, s.DetectedEntityType, s.DetectedEntityId })
                .Select(g => new PopularPage
                {
                    PageUrl = g.Key.PageUrl,
                    EntityType = g.Key.DetectedEntityType,
                    EntityId = g.Key.DetectedEntityId,
                    TotalVisits = g.Sum(s => s.TotalVisits),
                    UniqueVisits = g.Sum(s => s.UniqueVisits)
                })
                .OrderByDescending(p => p.TotalVisits)
                .ThenByDescending(p => p.UniqueVisits)
                .Take(top)
                .ToListAsync();

            // اضافه کردن رتبه
            for (int i = 0; i < popularPages.Count; i++)
            {
                popularPages[i].Rank = i + 1;
            }

            return popularPages;
        }

        public async Task<List<SectionStats>> GetSectionStatsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _dailyStatsRepository.TableNoTracking.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(s => s.StatDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(s => s.StatDate <= toDate.Value);

            var totalVisits = await query.SumAsync(s => s.TotalVisits);

            var sectionStats = await query
                .GroupBy(s => s.DetectedSection)
                .Select(g => new SectionStats
                {
                    Section = g.Key,
                    TotalVisits = g.Sum(s => s.TotalVisits),
                    UniqueVisits = g.Sum(s => s.UniqueVisits)
                })
                .OrderByDescending(s => s.TotalVisits)
                .ToListAsync();

            // محاسبه درصد
            foreach (var stat in sectionStats)
            {
                stat.Percentage = totalVisits > 0 ? (decimal)stat.TotalVisits / totalVisits * 100 : 0;
            }

            return sectionStats;
        }

        public async Task<List<DailyChartData>> GetDailyChartDataAsync(string entityType = null, long? entityId = null, int days = 30)
        {
            var toDate = DateTime.UtcNow.Date;
            var fromDate = toDate.AddDays(-days + 1);

            var query = _dailyStatsRepository.TableNoTracking
                .Where(s => s.StatDate >= fromDate && s.StatDate <= toDate);

            if (!string.IsNullOrEmpty(entityType))
                query = query.Where(s => s.DetectedEntityType == entityType);

            if (entityId.HasValue)
                query = query.Where(s => s.DetectedEntityId == entityId);

            var chartData = await query
                .GroupBy(s => s.StatDate)
                .Select(g => new DailyChartData
                {
                    Date = g.Key,
                    Visits = g.Sum(s => s.TotalVisits),
                    UniqueVisits = g.Sum(s => s.UniqueVisits)
                })
                .OrderBy(d => d.Date)
                .ToListAsync();

            // پر کردن تاریخ‌های missing
            var fullChartData = new List<DailyChartData>();
            for (var date = fromDate; date <= toDate; date = date.AddDays(1))
            {
                var existingData = chartData.FirstOrDefault(d => d.Date == date);
                fullChartData.Add(existingData ?? new DailyChartData
                {
                    Date = date,
                    Visits = 0,
                    UniqueVisits = 0
                });
            }

            return fullChartData;
        }

        public async Task<ServiceResult> CleanupOldDataAsync(int keepDays = 365)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.Date.AddDays(-keepDays);

                // پاک‌سازی داده‌های خام قدیمی
                var oldPageVisits = await _pageVisitRepository.Table
                    .Where(v => v.VisitDate < cutoffDate)
                    .ToListAsync();

                _pageVisitRepository.DeleteRange(oldPageVisits);

                // پاک‌سازی آمار روزانه قدیمی
                var oldDailyStats = await _dailyStatsRepository.Table
                    .Where(s => s.StatDate < cutoffDate)
                    .ToListAsync();

                _dailyStatsRepository.DeleteRange(oldDailyStats);

            

                _logger.LogInformation("Cleaned up {PageVisitsCount} page visits and {DailyStatsCount} daily stats older than {CutoffDate}",
                    oldPageVisits.Count, oldDailyStats.Count, cutoffDate.ToString("yyyy-MM-dd"));

                return ServiceResult.Ok($"داده‌های قدیمی تر از {cutoffDate:yyyy-MM-dd} با موفقیت پاک شدند");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up old data");
                return ServiceResult.Fail($"خطا در پاک‌سازی داده‌های قدیمی: {ex.Message}");
            }
        }

        public async Task<VisitSummary> GetVisitSummaryAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _dailyStatsRepository.TableNoTracking.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(s => s.StatDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(s => s.StatDate <= toDate.Value);

            var totalVisits = await query.SumAsync(s => s.TotalVisits);
            var uniqueVisits = await query.SumAsync(s => s.UniqueVisits);
            var totalPages = await query.Select(s => s.PageUrl).Distinct().CountAsync();

            var popularPages = await GetPopularPagesAsync(5, fromDate, toDate);
            var sectionStats = await GetSectionStatsAsync(fromDate, toDate);

            // محاسبه میانگین بازدید روزانه
            var daysCount = await query.Select(s => s.StatDate).Distinct().CountAsync();
            var avgVisitsPerDay = daysCount > 0 ? (decimal)totalVisits / daysCount : 0;

            return new VisitSummary
            {
                TotalVisits = totalVisits,
                UniqueVisits = uniqueVisits,
                TotalPages = totalPages,
                AvgVisitsPerDay = avgVisitsPerDay,
                MostPopularPage = popularPages.FirstOrDefault(),
                MostActiveSection = sectionStats.FirstOrDefault()?.Section,
                TopEntities = await GetTopEntitiesAsync(5, fromDate, toDate)
            };
        }

        // متدهای کمکی خصوصی
        private async Task<decimal> CalculateGrowthRateAsync(string entityType, long? entityId, DateTime? fromDate, DateTime? toDate)
        {
            if (!fromDate.HasValue || !toDate.HasValue)
                return 0;

            var currentPeriodDays = (toDate.Value - fromDate.Value).Days;
            if (currentPeriodDays < 7) // فقط برای بازه‌های زمانی معقول
                return 0;

            var previousFromDate = fromDate.Value.AddDays(-currentPeriodDays);
            var previousToDate = fromDate.Value.AddDays(-1);

            var currentStats = await GetEntityStatsAsync(entityType, entityId, fromDate, toDate);
            var previousStats = await GetEntityStatsAsync(entityType, entityId, previousFromDate, previousToDate);

            if (previousStats.TotalVisits == 0)
                return currentStats.TotalVisits > 0 ? 100 : 0;

            return ((decimal)(currentStats.TotalVisits - previousStats.TotalVisits) / previousStats.TotalVisits) * 100;
        }

        private async Task<List<DailyVisitData>> GetDailyVisitDataAsync(string entityType, long? entityId, DateTime? fromDate, DateTime? toDate)
        {
            var query = _dailyStatsRepository.TableNoTracking
                .Where(s => s.DetectedEntityType == entityType);

            if (entityId.HasValue)
                query = query.Where(s => s.DetectedEntityId == entityId);

            if (fromDate.HasValue)
                query = query.Where(s => s.StatDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(s => s.StatDate <= toDate.Value);

            return await query
                .GroupBy(s => s.StatDate)
                .Select(g => new DailyVisitData
                {
                    Date = g.Key,
                    Visits = g.Sum(s => s.TotalVisits),
                    UniqueVisits = g.Sum(s => s.UniqueVisits)
                })
                .OrderBy(d => d.Date)
                .ToListAsync();
        }

        private async Task<List<VisitSource>> GetVisitSourcesAsync(string pageUrl, DateTime? fromDate, DateTime? toDate)
        {
            var query = _pageVisitRepository.TableNoTracking
                .Where(v => v.PageUrl == pageUrl && !string.IsNullOrEmpty(v.Referrer));

            if (fromDate.HasValue)
                query = query.Where(v => v.VisitDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(v => v.VisitDate <= toDate.Value);

            var totalVisits = await query.CountAsync();

            var sources = await query
                .GroupBy(v => v.Referrer)
                .Select(g => new VisitSource
                {
                    Referrer = g.Key,
                    Visits = g.Count()
                })
                .OrderByDescending(s => s.Visits)
                .Take(10)
                .ToListAsync();

            // محاسبه درصد
            foreach (var source in sources)
            {
                source.Percentage = totalVisits > 0 ? (decimal)source.Visits / totalVisits * 100 : 0;
            }

            return sources;
        }

        private async Task<List<EntityVisitStats>> GetTopEntitiesAsync(int top, DateTime? fromDate, DateTime? toDate)
        {
            var query = _dailyStatsRepository.TableNoTracking.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(s => s.StatDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(s => s.StatDate <= toDate.Value);

            return await query
                .Where(s => !string.IsNullOrEmpty(s.DetectedEntityType))
                .GroupBy(s => new { s.DetectedEntityType, s.DetectedEntityId })
                .Select(g => new EntityVisitStats
                {
                    EntityType = g.Key.DetectedEntityType,
                    EntityId = g.Key.DetectedEntityId,
                    TotalVisits = g.Sum(s => s.TotalVisits),
                    UniqueVisits = g.Sum(s => s.UniqueVisits)
                })
                .OrderByDescending(e => e.TotalVisits)
                .Take(top)
                .ToListAsync();
        }
    }
}
