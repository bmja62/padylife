namespace Application.Plans.DTOs
{
    public class UserPlanStatsChartDTO
    {
        public long PlanId { get; set; }
        public string PlanTitle { get; set; }

        public int TotalQuestions { get; set; }
        public int AnswerCount { get; set; }

        public int TotalPoints { get; set; }
        public double ParticipationPercent => TotalQuestions == 0 ? 0 : Math.Round((double)AnswerCount * 100 / TotalQuestions, 1);
    }

}
