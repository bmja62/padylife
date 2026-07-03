using System.ComponentModel.DataAnnotations;

namespace PadyLife.Api.Models.Users
{
    /// <summary>
    /// مدل ساخت کاربر جدید
    /// </summary>
    public class RegisterDTO
    {
        /// <summary>
        /// نام کاربری
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// ایمیل
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// شماره همراه
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// گذرواژه
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// نوع ثبت نام (کاربر یا متخصص) توسط ثبتام کننده
        /// </summary>
        public UserType Type { get; set; }
        /// <summary>
        /// کد تأیید ارسال شده به شماره موبایل
        /// </summary>
        public string VerificationCode { get; set; }
    }
    /// <summary>
    /// نوع کاربر 
    /// </summary>
    public enum UserType
    {
        /// <summary>
        /// متخصص
        /// </summary>
        [Display(Name = "متخصص")]
        Specialist,
        /// <summary>
        /// کاربر
        /// </summary>
        [Display(Name = "کاربر")]
        User
    }

    /// <summary>
    /// مدل افزودن نقش به کاربر
    /// </summary>
    public class AddRoleToUserDTO
    {
        /// <summary>
        /// َناسه کاربر
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// نقش ها
        /// لیستی از رشته ها که نقش کاربر می باشد
        /// </summary>
        public List<string> Roles { get; set; }
    }

    public class SetPasswordRequest
    {
        public string NewPassword { get; set; }
    }

    public class GoogleLoginRequest
    {
        public string Credential { get; set; }
    }

    public class ForgotPasswordRequestDTO
    {
        public string PhoneNumber { get; set; }
    }

    public class ResetPasswordRequestDTO
    {
        public string PhoneNumber { get; set; }
        public string ResetCode { get; set; }
        public string NewPassword { get; set; }
    }

    public class ResetCodeData
    {
        public string Code { get; set; }
        public long UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// مدل درخواست ارسال کد تأیید
    /// </summary>
    public class SendVerificationCodeRequest
    {
        /// <summary>
        /// شماره همراه
        /// </summary>
        public string PhoneNumber { get; set; }
    }
}
