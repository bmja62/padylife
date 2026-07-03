using Services.Services.KavenegarServices.Dto;

namespace Services.Services.KavenegarServices
{
    public interface IKavenegarService
    {
        Task<SendResult> Send(string receptor, string message, string sender, CancellationToken cancellationToken);
        Task<List<SendResult>> SendArray(List<string> receptors, List<string> messages, string sender , CancellationToken cancellationToken );
        Task<StatusResult> Status(string messageId, CancellationToken cancellationToken );
        Task<StatusResult> StatusByLocalId(string localId, CancellationToken cancellationToken );
        Task<List<SendResult>> SelectOutbox(DateTime? startDate , DateTime? endDate , string sender , CancellationToken cancellationToken );
        Task<List<SendResult>> LatestOutBox(int pageSize , string sender , CancellationToken cancellationToken );
        Task<SendResult> Lookup(string receptor, string token, string template, CancellationToken cancellationToken );
        Task<SendResult> SendWithTemplate(string receptor, string template, CancellationToken cancellationToken , params string[] tokenValues);
        Task<List<SendResult>> SendWithTemplateArray(List<string> receptors, string template, CancellationToken cancellationToken, params string[] tokenValues);
        Task<SendResult> TTS(string receptor, string message, CancellationToken cancellationToken );
    }
}
