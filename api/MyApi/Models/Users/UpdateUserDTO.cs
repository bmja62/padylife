using Entities.Users;

namespace PadyLife.Api.Models.Users
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateUserDTO
    {
        /// <summary>
        /// نام و نام خانوادگی
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// سن
        /// </summary>
        public int Age { get; set; }
        /// <summary>
        /// جنسیت
        /// </summary>
        public GenderType Gender { get; set; }
        /// <summary>
        /// وضعیت فعالیت
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// تاریخ تولد
        /// </summary>
        public DateTime? Birthdate { get; set; }
        /// <summary>
        /// قد
        /// </summary>
        public int? Hight { get; set; }
        /// <summary>
        /// وزن
        /// </summary>
        public int? Wight { get; set; }
        /// <summary>
        /// شغل
        /// </summary>
        public string JobTitle { get; set; }
        /// <summary>
        /// وضعیت تاهل
        /// </summary>
        public MaritalStatus MaritalStatus { get; set; }
        /// <summary>
        /// اینستاگرام
        /// </summary>
        public string InstagramId { get; set; }
        /// <summary>
        /// نمایه تصویری فرد
        /// </summary>
        public string ProfileImage { get; set; }
    }
}
