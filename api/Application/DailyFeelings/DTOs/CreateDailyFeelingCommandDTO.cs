using Entities.DailyFeelings;

namespace Application.DailyFeelings.DTOs
{
    public class CreateDailyFeelingCommandDTO
    {
        //Props
        /// <summary>
        /// احساسات فرد
        /// </summary>
        public DailyFeelingsType Type { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// صدای فرد
        /// </summary>
        public string VoiceUrl { get; set; }
    }
}
