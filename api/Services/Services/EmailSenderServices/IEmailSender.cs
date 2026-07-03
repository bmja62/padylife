namespace Services.Services.EmailSenderServices
{
    public interface IEmailSender
    {
        Task<ServiceResult> Send(string email, string subject, string body);
    }
}
