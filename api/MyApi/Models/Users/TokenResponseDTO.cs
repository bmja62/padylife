using Services;

namespace PadyLife.Api.Models.Users
{
    /// <summary>
    /// پاسخ درخواست توکن
    /// </summary>
    public class TokenResponseDTO
    {
        /// <summary>
        /// اطلاعات کاربری
        /// </summary>
        public UserDTO User { get; set; }
        /// <summary>
        /// توکن
        /// </summary>
        public AccessToken AccessToken { get; set; }
    }
}
