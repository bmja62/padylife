using Entities.Common;
using Entities.Users;
using System.ComponentModel.DataAnnotations;

namespace Entities.DailyFeelings
{
    public class DailyFeeling : BaseEntity<long>
    {
        public DailyFeeling(DailyFeelingsType type, string description, string voiceUrl, long userId)
        {
            Type = type;
            Description = description;
            VoiceUrl = voiceUrl;
            CreatedByUserId = userId;
        }

        private DailyFeeling()
        {

        }

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

        //FKs
        /// <summary>
        /// ساخته شده توسط
        /// </summary>
        public long CreatedByUserId { get; set; }

        //Navigations
        public User CreatedByUser { get; set; }

        //Factory Methods
        public static DailyFeeling CreateDefault(DailyFeelingsType type, string description, string voiceUrl, long userId) => new(type, description, voiceUrl, userId);

    }

    public enum DailyFeelingsType
    {
        [Display(Name = "توپ")]
        Glad = 1,
        [Display(Name = "خوشحال")]
        Happy = 2,
        [Display(Name = "عادی")]
        Poker = 3,
        [Display(Name = "ناراحت")]
        Sad = 4,
        [Display(Name = "داغون")]
        Bad = 5,
    }
}
