using Application.Cqrs.Commands;
using Services;

namespace Application.Visits.Commands
{
    // Application/Visits/Commands/ProcessDateRangeStatsCommand.cs
    public class ProcessDateRangeStatsCommand : ICommand<ServiceResult>
    {
        public DateTime FromDate { get; }
        public DateTime ToDate { get; }

        public ProcessDateRangeStatsCommand(DateTime fromDate, DateTime toDate)
        {
            FromDate = fromDate;
            ToDate = toDate;
        }
    }
}
