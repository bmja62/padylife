namespace PadyLife.Api.Models.Users
{
    /// <summary>
    /// درخواست توکن
    /// </summary>
    public class TokenRequest
    {
        /// <summary>
        /// نام کاربری
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// گذر واژه
        /// </summary>
        public string password { get; set; }
    }
}
