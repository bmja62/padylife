namespace Services.Services.EmailSenderServices.DTOs
{
    public class EmailConfigDTO
    {
        public string HostIp { get; set; }
        public int Port { get; set; }
        public string SenderEmail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsSSL { get; set; }
    }
}
