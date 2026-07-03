namespace Application.Reports.DTOs
{
    public class PerformanceChartPointDTO
    {
        public string PeriodLabel { get; set; } // مثلاً "Week 1", "2025-05"
        public int AnswerCount { get; set; }
        public int EarnedPoints { get; set; }
        public string Trend { get; set; } // 📈 📉 ➖
    }
}
