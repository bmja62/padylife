namespace Services.Services.NotifyServices.DTOs
{
    public class GetByTokenDTO
    {
        public long UserId { get; internal set; }
        public string FullName { get; internal set; }
        public string Email { get; internal set; }
        public string PhoneNumber { get; internal set; }
        public int UnReadNotificationCount { get; internal set; }
    }
}
