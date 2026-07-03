using Application.Cqrs.Commands;
using Services;

namespace Application.Visits.Commands
{
    // Application/Visits/Commands/CleanupOldDataCommand.cs
    public class CleanupOldDataCommand : ICommand<ServiceResult>
    {
        public int KeepDays { get; }

        public CleanupOldDataCommand(int keepDays)
        {
            KeepDays = keepDays;
        }
    }
}
