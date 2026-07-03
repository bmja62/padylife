namespace Entities.Addresses
{
    using Entities.Baskets;
    using Entities.Common;
    using Entities.Locations;
    using Entities.Orders;
    using Entities.Users;
    using System.ComponentModel.DataAnnotations;

    namespace ECommerce.Entities
    {
        public class Address : BaseEntity<long>
        {
            //FKs

            /// <summary>
            /// کاربر
            /// </summary>
            public long UserId { get; set; }

            /// <summary>
            /// کشور
            /// </summary>
            [Required(ErrorMessage = "کشور الزامی است")]
            public long CountryId { get; set; }

            /// <summary>
            /// استان
            /// </summary>
            [Required(ErrorMessage = "استان الزامی است")]
            public long ProvinceId { get; set; }

            /// <summary>
            /// شهر
            /// </summary>
            [Required(ErrorMessage = "شهر الزامی است")]
            public long CityId { get; set; }


            //Props 

            /// <summary>
            /// کدپستی
            /// </summary>
            [RegularExpression(@"\d{10}", ErrorMessage = "کد پستی باید 10 رقم باشد")]
            public string PostalCode { get; set; }

            /// <summary>
            /// آدرس
            /// </summary>
            [Required(ErrorMessage = "متن آدرس الزامی است")]
            [StringLength(500, ErrorMessage = "آدرس نمی‌تواند بیش از 500 کاراکتر باشد")]
            public string AddressText { get; set; }

            /// <summary>
            /// پلاک
            /// </summary>
            public int? Plaque { get; set; }
            /// <summary>
            /// واحد
            /// </summary>
            public int? Unit { get; set; }
            /// <summary>
            /// طبقه
            /// </summary>
            public int? Floor { get; set; }

            /// <summary>
            /// نام گیرنده
            /// </summary>
            [Required(ErrorMessage = "نام گیرنده الزامی است")]
            [StringLength(100, ErrorMessage = "نام گیرنده نمی‌تواند بیش از 100 کاراکتر باشد")]
            public string RecipientName { get; set; }

            /// <summary>
            /// تلفن همراه گیرنده
            /// </summary>
            [Required(ErrorMessage = "تلفن همراه گیرنده الزامی است")]
            [RegularExpression(@"^09\d{9}$", ErrorMessage = "شماره تلفن همراه معتبر نیست")]
            public string RecipientPhone { get; set; }

            /// <summary>
            /// نلفن ثابت گیرنده
            /// </summary>
            [RegularExpression(@"^0\d{10}$", ErrorMessage = "شماره تلفن ثابت معتبر نیست")]
            public string LandlinePhone { get; set; }

            /// <summary>
            /// آدرس همیشگی
            /// </summary>
            public bool IsDefault { get; set; } = false;

            /// <summary>
            /// مکان آدرس محل کار یا خانه و...
            /// </summary>
            public AddressType? AddressType { get; set; }

            /// <summary>
            /// موقعیت مکانی
            /// </summary>
            public string GeoLocation { get; set; } // می‌تواند به صورت "lat,long" ذخیره شود

            //Navigations
            public User User { get; set; }
            public Country Country { get; set; }
            public Province Province { get; set; }
            public City City { get; set; }

            public ICollection<Basket> Baskets { get; set; }
            public ICollection<Order> Orders { get; set; }

            // متدهای کمکی
            public string GetFullAddress()
            {
                var countryNameFa = Country != null ? Country?.CountryNameFa : "-";
                var provinceNameFa = Province != null ? Province?.ProvinceNameFa : "-";
                var cityNameFa = City != null ? City?.CityNameFa : "-";
                return $"{countryNameFa}، {provinceNameFa}، {cityNameFa}، {AddressText}، پلاک {Plaque}، واحد {Unit}، کدپستی {PostalCode}";
            }

            public void SetAsDefault()
            {
                IsDefault = true;
                // در اینجا می‌توانید سایر آدرس‌های کاربر را از حالت پیش‌فرض خارج کنید
            }
        }
    }
}
