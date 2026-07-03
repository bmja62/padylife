namespace Application.Reports.DTOs
{
    public class GroupPerformanceChartSeriesDTO
    {
        public long UserId { get; set; }
        public string UserName { get; set; }

        public List<PerformanceChartPointDTO> DataPoints { get; set; }
    }

}
