namespace Services.Services.EmailTemplateService
{
    public interface IEmailTemplateService
    {
        string GetActiveCodeTemplate(string code);
        string GetNewGroupClassReserveTemplate(string subject);
        string GetNewReserveTemplate(string datetimesArrays);
    }
}
