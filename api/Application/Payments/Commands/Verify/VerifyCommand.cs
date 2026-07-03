using Application.Cqrs.Commands;
using Application.Payments.DTOs;
using Services;

namespace Application.Payments.Commands.Verify
{
    public class VerifyCommand(long trackId) : ICommand<ServiceResult<VerifyResultDTO>>
    {
        public long TrackId { get; } = trackId;
    }
}
