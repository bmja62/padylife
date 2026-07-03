namespace Common.Utilities
{
    public static class NumberToWordsConverterExtentions
    {
        static string[] ones = { "", "یک", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه", "ده", "یازده", "دوازده", "سیزده", "چهارده", "پانزده", "شانزده", "هفده", "هجده", "نوزده" };
        static string[] tens = { "", "", "بیست", "سی", "چهل", "پنجاه", "شصت", "هفتاد", "هشتاد", "نود" };
        static string[] hundreds = { "", "صد", "دویست", "سیصد", "چهارصد", "پانصد", "ششصد", "هفتصد", "هشتصد", "نهصد" };
        static string[] bigs = { "", "هزار", "میلیون", "میلیارد" };

        public static string Convert(this long number)
        {
            if (number == 0)
                return "صفر";

            string result = "";
            int group = 0;

            while (number > 0)
            {
                int threeDigits = (int)(number % 1000);
                if (threeDigits != 0)
                {
                    string groupText = ConvertThreeDigits(threeDigits);
                    if (!string.IsNullOrEmpty(bigs[group]))
                        groupText += " " + bigs[group];

                    result = groupText + (string.IsNullOrEmpty(result) ? "" : " و " + result);
                }

                number /= 1000;
                group++;
            }

            return result;
        }

        private static string ConvertThreeDigits(this int number)
        {
            List<string> parts = new List<string>();

            int h = number / 100;
            int rem = number % 100;

            if (h > 0)
                parts.Add(hundreds[h]);

            if (rem > 0)
            {
                if (rem < 20)
                    parts.Add(ones[rem]);
                else
                {
                    int t = rem / 10;
                    int o = rem % 10;
                    parts.Add(tens[t]);
                    if (o > 0)
                        parts.Add(ones[o]);
                }
            }

            return string.Join(" و ", parts);
        }
    }
}
