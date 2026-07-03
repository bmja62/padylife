using System.Text.RegularExpressions;

namespace Common.Utilities.Helpers
{
    public static class UsernameValidator
    {
        public static bool ValidateUsername(this string username)
        {
            // الگوی Regex: فقط حروف انگلیسی، اعداد و زیرخط (_) مجاز هستند
            const string pattern = @"^[a-zA-Z0-9_]+$";
            return Regex.IsMatch(username, pattern);
        }
    }
}
