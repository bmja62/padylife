using Data.Contracts;
using Entities.Locations;

namespace Services.DataInitializer
{
    public class LocationInitializer(
        IRepository<Country> countryRepo,
        IRepository<Province> provinceRepo,
        IRepository<City> cityRepo) : IDataInitializer
    {
        public void InitializeData()
        {
            if (countryRepo.Table.Any())
                return;

            // افزودن ایران
            var iran = AddCountry("Iran", "ایران", "IR", "98");

            // افزودن تمام استان‌های ایران
            var provinces = new List<(string name, string nameFa, string code, List<(string name, string nameFa, string code)> cities)>
            {
                ("Tehran", "تهران", "021", new List<(string, string, string)> {
                    ("Tehran", "تهران", "021"), ("Karaj", "کرج", "026"), ("Varamin", "ورامین", "021"),
                    ("Shahriar", "شهریار", "021"), ("Robat Karim", "رباط کریم", "021")
                }),
                ("Isfahan", "اصفهان", "031", new List<(string, string, string)> {
                    ("Isfahan", "اصفهان", "031"), ("Kashan", "کاشان", "031"), ("Najafabad", "نجف آباد", "031"),
                    ("Shahreza", "شهرضا", "031"), ("Golpayegan", "گلپایگان", "031")
                }),
                ("Fars", "فارس", "071", new List<(string, string, string)> {
                    ("Shiraz", "شیراز", "071"), ("Marvdasht", "مرودشت", "071"), ("Jahrom", "جهرم", "071"),
                    ("Kazerun", "کازرون", "071"), ("Fasa", "فسا", "071")
                }),
                ("Khorasan Razavi", "خراسان رضوی", "051", new List<(string, string, string)> {
                    ("Mashhad", "مشهد", "051"), ("Neyshabur", "نیشابور", "051"), ("Sabzevar", "سبزوار", "051"),
                    ("Torbat-e Heydarieh", "تربت حیدریه", "051"), ("Quchan", "قوچان", "051")
                }),
                ("East Azerbaijan", "آذربایجان شرقی", "041", new List<(string, string, string)> {
                    ("Tabriz", "تبریز", "041"), ("Maragheh", "مراغه", "041"), ("Marand", "مرند", "041"),
                    ("Ahar", "اهر", "041"), ("Mianeh", "میانه", "041")
                }),
                ("West Azerbaijan", "آذربایجان غربی", "044", new List<(string, string, string)> {
                    ("Urmia", "ارومیه", "044"), ("Khoy", "خوی", "044"), ("Mahabad", "مهاباد", "044"),
                    ("Miandoab", "میاندوآب", "044"), ("Piranshahr", "پیرانشهر", "044")
                }),
                ("Kermanshah", "کرمانشاه", "083", new List<(string, string, string)> {
                    ("Kermanshah", "کرمانشاه", "083"), ("Islamabad-e Gharb", "اسلام آباد غرب", "083"),
                    ("Harsin", "هرسین", "083"), ("Kangavar", "کنگاور", "083"), ("Sonqor", "سنقر", "083")
                }),
                ("Khuzestan", "خوزستان", "061", new List<(string, string, string)> {
                    ("Ahvaz", "اهواز", "061"), ("Abadan", "آبادان", "061"), ("Khorramshahr", "خرمشهر", "061"),
                    ("Dezful", "دزفول", "061"), ("Andimeshk", "اندیمشک", "061")
                }),
                ("Gilan", "گیلان", "013", new List<(string, string, string)> {
                    ("Rasht", "رشت", "013"), ("Bandar Anzali", "بندر انزلی", "013"), ("Lahijan", "لاهیجان", "013"),
                    ("Langroud", "لنگرود", "013"), ("Astara", "آستارا", "013")
                }),
                ("Mazandaran", "مازندران", "011", new List<(string, string, string)> {
                    ("Sari", "ساری", "011"), ("Babol", "بابل", "011"), ("Amol", "آمل", "011"),
                    ("Qaem Shahr", "قائم شهر", "011"), ("Nowshahr", "نوشهر", "011")
                }),
                // استان‌های دیگر...
                ("Markazi", "مرکزی", "086", new List<(string, string, string)> {
                    ("Arak", "اراک", "086"), ("Saveh", "ساوه", "086"), ("Khomein", "خمین", "086"),
                    ("Mahallat", "محلات", "086"), ("Delijan", "دلیجان", "086")
                }),
                ("Hormozgan", "هرمزگان", "076", new List<(string, string, string)> {
                    ("Bandar Abbas", "بندر عباس", "076"), ("Minab", "میناب", "076"), ("Qeshm", "قشم", "076"),
                    ("Kish", "کیش", "076"), ("Hajiabad", "حاجی آباد", "076")
                }),
                ("Kerman", "کرمان", "034", new List<(string, string, string)> {
                    ("Kerman", "کرمان", "034"), ("Sirjan", "سیرجان", "034"), ("Rafsanjan", "رفسنجان", "034"),
                    ("Bam", "بم", "034"), ("Jiroft", "جیرفت", "034")
                }),
                ("Yazd", "یزد", "035", new List<(string, string, string)> {
                    ("Yazd", "یزد", "035"), ("Mehriz", "مهریز", "035"), ("Ardakan", "اردکان", "035"),
                    ("Bafq", "بافق", "035"), ("Taft", "تفت", "035")
                }),
                ("Ardabil", "اردبیل", "045", new List<(string, string, string)> {
                    ("Ardabil", "اردبیل", "045"), ("Meshgin Shahr", "مشگین شهر", "045"), ("Parsabad", "پارس آباد", "045"),
                    ("Germi", "گرمی", "045"), ("Namin", "نمین", "045")
                }),
                ("Bushehr", "بوشهر", "077", new List<(string, string, string)> {
                    ("Bushehr", "بوشهر", "077"), ("Borazjan", "برازجان", "077"), ("Ganaveh", "گناوه", "077"),
                    ("Dashti", "دشتی", "077"), ("Tangestan", "تنگستان", "077")
                }),
                ("Zanjan", "زنجان", "024", new List<(string, string, string)> {
                    ("Zanjan", "زنجان", "024"), ("Abhar", "ابهر", "024"), ("Khodabandeh", "خدابنده", "024"),
                    ("Mahneshan", "ماهنشان", "024"), ("Tarom", "طارم", "024")
                }),
                ("Semnan", "سمنان", "023", new List<(string, string, string)> {
                    ("Semnan", "سمنان", "023"), ("Shahrud", "شاهرود", "023"), ("Damghan", "دامغان", "023"),
                    ("Garmsar", "گرمسار", "023"), ("Aradan", "آرادان", "023")
                }),
                ("Qom", "قم", "025", new List<(string, string, string)> {
                    ("Qom", "قم", "025"), ("Qanavat", "قنوات", "025"), ("Jafarieh", "جعفریه", "025"),
                    ("Kahak", "کهک", "025"), ("Salafchegan", "سلفچگان", "025")
                }),
                ("Golestan", "گلستان", "017", new List<(string, string, string)> {
                    ("Gorgan", "گرگان", "017"), ("Gonbad-e Kavus", "گنبد کاووس", "017"), ("Bandar Gaz", "بندر گز", "017"),
                    ("Aliabad", "علی آباد", "017"), ("Kordkuy", "کردکوی", "017")
                }),
                ("Kurdistan", "کردستان", "087", new List<(string, string, string)> {
                    ("Sanandaj", "سنندج", "087"), ("Saqqez", "سقز", "087"), ("Marivan", "مریوان", "087"),
                    ("Baneh", "بانه", "087"), ("Divandarreh", "دیواندره", "087")
                }),
                ("Hamadan", "همدان", "081", new List<(string, string, string)> {
                    ("Hamadan", "همدان", "081"), ("Malayer", "ملایر", "081"), ("Nahavand", "نهاوند", "081"),
                    ("Tuyserkan", "تویسرکان", "081"), ("Kabudarahang", "کبودرآهنگ", "081")
                }),
                ("Chaharmahal and Bakhtiari", "چهارمحال و بختیاری", "038", new List<(string, string, string)> {
                    ("Shahr-e Kord", "شهرکرد", "038"), ("Borujen", "بروجن", "038"), ("Farsan", "فارسان", "038"),
                    ("Lordegan", "لردگان", "038"), ("Kiar", "کیار", "038")
                }),
                ("Lorestan", "لرستان", "066", new List<(string, string, string)> {
                    ("Khorramabad", "خرم آباد", "066"), ("Borujerd", "بروجرد", "066"), ("Aligudarz", "الیگودرز", "066"),
                    ("Dorud", "دورود", "066"), ("Kuhdasht", "کوهدشت", "066")
                }),
                ("Ilam", "ایلام", "084", new List<(string, string, string)> {
                    ("Ilam", "ایلام", "084"), ("Mehran", "مهران", "084"), ("Dehloran", "دهلران", "084"),
                    ("Ivan", "ایوان", "084"), ("Abdanan", "آبدانان", "084")
                }),
                ("Kohgiluyeh and Boyer-Ahmad", "کهگیلویه و بویراحمد", "074", new List<(string, string, string)> {
                    ("Yasuj", "یاسوج", "074"), ("Gachsaran", "گچساران", "074"), ("Dehdasht", "دهدشت", "074"),
                    ("Landeh", "لنده", "074"), ("Basht", "باشت", "074")
                }),
                ("North Khorasan", "خراسان شمالی", "058", new List<(string, string, string)> {
                    ("Bojnord", "بجنورد", "058"), ("Shirvan", "شیروان", "058"), ("Esfarayen", "اسفراین", "058"),
                    ("Garmeh", "گرمه", "058"), ("Jajarm", "جاجرم", "058")
                }),
                ("South Khorasan", "خراسان جنوبی", "056", new List<(string, string, string)> {
                    ("Birjand", "بیرجند", "056"), ("Qaen", "قائن", "056"), ("Ferdows", "فردوس", "056"),
                    ("Nehbandan", "نهبندان", "056"), ("Sarayan", "سرایان", "056")
                }),
                ("Alborz", "البرز", "026", new List<(string, string, string)> {
                    ("Karaj", "کرج", "026"), ("Nazarabad", "نظرآباد", "026"), ("Eshtehard", "اشتهارد", "026"),
                    ("Taleqan", "طالقان", "026"), ("Savojbolagh", "ساوجبلاغ", "026")
                }),
                ("Sistan and Baluchestan", "سیستان و بلوچستان", "054", new List<(string, string, string)> {
                    ("Zahedan", "زاهدان", "054"), ("Zabol", "زابل", "054"), ("Chabahar", "چابهار", "054"),
                    ("Iranshahr", "ایرانشهر", "054"), ("Saravan", "سراوان", "054")
                })
            };

            // افزودن استان‌ها و شهرهای ایران
            foreach (var province in provinces)
            {
                var provinceEntity = AddProvince(iran.Id, province.name, province.nameFa, province.code);

                foreach (var city in province.cities)
                {
                    AddCity(provinceEntity.Id, city.name, city.nameFa, city.code);
                }
            }
        }

        private Country AddCountry(string name, string nameFa, string code, string phoneCode)
        {
            var country = new Country
            {
                CountryName = name,
                CountryNameFa = nameFa,
                CountryCode = code,
                PhoneCode = phoneCode,
                IsActive = true
            };
            countryRepo.Add(country);
            return country;
        }

        private Province AddProvince(long countryId, string name, string nameFa, string code)
        {
            var province = new Province
            {
                CountryId = countryId,
                ProvinceName = name,
                ProvinceNameFa = nameFa,
                ProvinceCode = code,
                IsActive = true
            };
            provinceRepo.Add(province);
            return province;
        }

        private City AddCity(long provinceId, string name, string nameFa, string code)
        {
            var city = new City
            {
                ProvinceId = provinceId,
                CityName = name,
                CityNameFa = nameFa,
                CityCode = code,
                IsActive = true
            };
            cityRepo.Add(city);
            return city;
        }
    }
}