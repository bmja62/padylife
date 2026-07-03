namespace Services.DataInitializer
{
    using Data.Contracts;
    using Entities.Calendar;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http; // لازم
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    namespace Services.DataInitializer
    {
        public class CalendarDataInitializer : IDataInitializer
        {
            private readonly IRepository<CalendarModel> _calendarRepo;
            private readonly IHttpClientFactory? _httpClientFactory; 
            private readonly JsonSerializerOptions _jsonOptions;

            public CalendarDataInitializer(IRepository<CalendarModel> calendarRepo,
                                           IHttpClientFactory? httpClientFactory = null)
            {
                _calendarRepo = calendarRepo;
                _httpClientFactory = httpClientFactory;

                _jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
            }

            public void InitializeData()
            {
                var currentYear = DateTime.Now.Year;
                var persianCalendar = new PersianCalendar();
                int currentPersianYear = persianCalendar.GetYear(DateTime.Now);
                for (int year = currentPersianYear - 1; year <= currentPersianYear + 10; year++)
                {
                    bool yearAlreadySeeded = _calendarRepo.TableNoTracking
                        .Any(c => c.ShamsiDate.StartsWith($"{year}/"));

                    if (yearAlreadySeeded)
                        continue;

                    var items = FetchYearAsync(year).GetAwaiter().GetResult();
                    if (items == null || items.Count == 0)
                        continue;

                    var calEntities = new List<CalendarModel>(items.Count);

                    foreach (var d in items)
                    {
                        // اگر shamsiDate خالی بود از date بردار
                        var shamsi = !string.IsNullOrWhiteSpace(d.ShamsiDate) ? d.ShamsiDate : d.Date;

                        // تبدیل شمسی به میلادی
                        var greg = ToGregorianDate(shamsi);

                        // شماره روز هفته 1..7
                        var weekdayNum = GetPersianWeekdayNumber(greg.DayOfWeek);

                        // جمعه‌ها تعطیل
                        bool isHolidayFinal = d.IsHoliday || greg.DayOfWeek == DayOfWeek.Friday;

                        calEntities.Add(new CalendarModel
                        {
                            Date = greg, // میلادی
                            ShamsiDate = shamsi, // "YYYY/MM/DD"
                            IsHoliday = isHolidayFinal,
                            HolidayDesription = d.HolidayDesription ?? string.Empty,
                            Weekday = weekdayNum // 1..7
                        });
                    }

                    // درج دسته‌ای
                    _calendarRepo.AddRange(calEntities);
                }
            }

            private async Task<List<DayDto>> FetchYearAsync(int year)
            {
                var url = $"https://api.persian-calendar.ir/api/v1/calendar/get-year/{year}";

                using var client = _httpClientFactory?.CreateClient(nameof(CalendarDataInitializer))
                                   ?? new HttpClient();
                using var resp = await client.GetAsync(url);
                resp.EnsureSuccessStatusCode();

                var json = await resp.Content.ReadAsStringAsync();
                var parsed = JsonSerializer.Deserialize<YearResponse>(json, _jsonOptions);
                return parsed?.Data ?? new List<DayDto>();
            }

            /// <summary>
            /// ورودی: "YYYY/MM/DD" شمسی → خروجی: DateTime میلادی (Kind=Unspecified)
            /// </summary>
            private static DateTime ToGregorianDate(string shamsiYmd)
            {
                var parts = shamsiYmd.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 3)
                    throw new FormatException($"Invalid Shamsi date format: {shamsiYmd}");

                int y = int.Parse(parts[0]);
                int m = int.Parse(parts[1]);
                int d = int.Parse(parts[2]);

                var pc = new PersianCalendar();
                var dt = pc.ToDateTime(y, m, d, 0, 0, 0, 0); // اینجا kind نمی‌گیرد
                return DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
            }

            private static int GetPersianWeekdayNumber(DayOfWeek dow) => dow switch
            {
                DayOfWeek.Saturday => 1, // شنبه
                DayOfWeek.Sunday => 2, // یکشنبه
                DayOfWeek.Monday => 3, // دوشنبه
                DayOfWeek.Tuesday => 4, // سه‌شنبه
                DayOfWeek.Wednesday => 5, // چهارشنبه
                DayOfWeek.Thursday => 6, // پنجشنبه
                DayOfWeek.Friday => 7, // جمعه
                _ => 0
            };
        }


        internal class YearResponse
        {
            [JsonPropertyName("data")]
            public List<DayDto> Data { get; set; } = new();
        }

        internal class DayDto
        {
            [JsonPropertyName("date")]
            public string Date { get; set; } = default!;

            [JsonPropertyName("shamsiDate")]
            public string ShamsiDate { get; set; } = default!;

            [JsonPropertyName("isHoliday")]
            public bool IsHoliday { get; set; }

            [JsonPropertyName("holidayDesription")]
            public string HolidayDesription { get; set; }
        }
    }
}
