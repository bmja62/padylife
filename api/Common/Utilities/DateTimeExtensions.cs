namespace Common.Utilities
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startDay)
        {
            int diff = (7 + (dt.DayOfWeek - startDay)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime StartOfMonth(this DateTime dt) => new DateTime(dt.Year, dt.Month, 1);
        public static DateTime EndOfMonth(this DateTime dt) => dt.StartOfMonth().AddMonths(1).AddDays(-1);
    }

}
