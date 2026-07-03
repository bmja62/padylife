using Entities.DailyFeelings;

namespace Application.DailyFeelings.DTOs
{
    public class GetAllDailyFeelingDTO
    {
        public long Id { get; internal set; }
        public DailyFeelingsType Type { get; internal set; }
        public string Description { get; internal set; }
        public string VoiceUrl { get; internal set; }
        public GetAllDailyFeelingUserInfoDTO UserInfo { get; internal set; }
        public DateTime CreatedAt { get; internal set; }
    }

    public class GetAllDailyFeelingUserInfoDTO
    {
        public long CreatedByUserId { get; internal set; }
        public string UserName { get; internal set; }
        public string FullName { get; internal set; }
        public int Age { get; internal set; }
        public string Email { get; internal set; }
    }
}
