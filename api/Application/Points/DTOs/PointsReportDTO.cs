namespace Application.Points.DTOs
{
    public class PointsReportDTO
    {
        public int MyTotalPoints { get; set; }
        public int MyRank { get; set; }
        public int TotalUsers { get; set; }
        public decimal MyPercentile { get; set; } // مثل 92.4 => یعنی بالاتر از 92.4% کاربران
        public List<PointsOverTimeDTO> History { get; set; }
    }
    //public class PointsOverTimeDTO
    //{
    //    public DateTime Date { get; set; }
    //    public int PointsEarned { get; set; }
    //    public string Resons { get; internal set; }
    //}
    public class PointsOverTimeDTO
    {
        public DateTime Date { get; set; }
        public int PointsEarned { get; set; }
        public List<HourlyDetailDTO> HourlyDetails { get; set; } = new();
    }

    public class HourlyDetailDTO
    {
        public string Time { get; set; } = string.Empty; // فرمت "HH:mm"
        public int Points { get; set; }
        public List<string> Reasons { get; set; } = new();
    }

 
}
