using Hangfire;
using Services.JobsServices;



namespace WebFramework.Configuration
{
    public static class HangfireJobsConfig
    {
        public static void RegisterRecurringJobs(IRecurringJobManager jobManager)
        {
            // Job قبلی برای بررسی پلن کاربران
            jobManager.AddOrUpdate<CheckUserPlansJob>(
                "check-user-plans",
                job => job.RunAsync(CancellationToken.None),
                "0 8 * * *" // هر روز ساعت 8 صبح
            );

            // Job جدید برای پردازش آمار روزانه - هر 10 دقیقه
            jobManager.AddOrUpdate<ProcessPer10MINStatsJob>(
                "process-per-ten-min-stats",
                job => job.RunAsync(CancellationToken.None),
                "*/10 * * * *" // هر 10 دقیقه
            );

            //// Job جدید برای پردازش آمار روزانه - هر روز 8 صبح
            //jobManager.AddOrUpdate<ProcessDailyStatsJob>(
            //    "process-daily-stats",
            //    job => job.RunAsync(CancellationToken.None),
            //    "0 8 * * *" // هر روز ساعت 8 صبح
            //);

            // Job جدید برای پاک‌سازی داده‌های قدیمی - هر هفته
            jobManager.AddOrUpdate<CleanupOldVisitDataJob>(
                "cleanup-old-visit-data",
                job => job.RunAsync(CancellationToken.None),
                "0 2 * * 0" // هر یکشنبه ساعت 2 بامداد
            );
        }
    }


}