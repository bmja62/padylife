namespace Application.Reports.DTOs
{
    public class TeamPerformanceChartSeriesDTO
    {
        public string PlanName { get; set; }
        public string PlanImageUrl { get; internal set; }
        public List<PerformanceChartPointDTO> DataPoints { get; set; }
    }
}
