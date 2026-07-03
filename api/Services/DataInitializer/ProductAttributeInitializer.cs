using Data.Contracts;
using Entities.Products;

namespace Services.DataInitializer
{
    internal class ProductAttributeInitializer(IRepository<ProductAttribute> repository) : IDataInitializer
    {
        public void InitializeData()
        {
            if (repository.Table.Any())
                return;

            var attributes = new List<ProductAttribute>();
            attributes.AddRange(GetTextualAttributes());
            attributes.AddRange(GetNumericAttributes());
            attributes.AddRange(GetBooleanAttributes());
            attributes.AddRange(GetColorAttributes());
            attributes.AddRange(GetDateTimeAttributes());
            attributes.AddRange(GetFileAttributes());
            attributes.AddRange(GetSelectionAttributes());
            attributes.AddRange(GetOtherAttributes());

            repository.AddRange(attributes);
        }

        private List<ProductAttribute> GetTextualAttributes() => new()
        {
            NewAttribute("عنوان", "عنوان محصول به صورت متن ساده", AttributeType.Text),
            NewAttribute("توضیحات کامل", "توضیحات با فرمت HTML", AttributeType.RichText),
            NewAttribute("برچسب‌ها", "برچسب‌های مرتبط با محصول", AttributeType.Tags),
        };

        private List<ProductAttribute> GetNumericAttributes() => new()
        {
            NewAttribute("تعداد موجودی", "تعداد کالا در انبار", AttributeType.Number),
            NewAttribute("وزن", "وزن محصول به کیلوگرم", AttributeType.Decimal),
            NewAttribute("درصد تخفیف", "درصد تخفیف اعمال شده", AttributeType.Percentage),
            NewAttribute("قیمت", "قیمت نهایی محصول با واحد پولی", AttributeType.Currency),
            NewAttribute("امتیاز کاربران", "میانگین امتیاز کاربران", AttributeType.Rating),
            NewAttribute("مدت گارانتی", "مدت زمان گارانتی به ماه", AttributeType.TimeDuration),
        };

        private List<ProductAttribute> GetBooleanAttributes() => new()
        {
            NewAttribute("موجود است", "آیا محصول موجود است یا خیر", AttributeType.Boolean),
        };

        private List<ProductAttribute> GetColorAttributes()
        {
            var colors = new[] { "قرمز", "آبی", "سبز", "زرد", "مشکی", "سفید", "خاکستری", "نارنجی", "بنفش", "صورتی", "قهوه‌ای", "زرشکی", "طلایی", "نقره‌ای" };
            var list = new List<ProductAttribute>();
            foreach (var color in colors)
            {
                list.Add(NewAttribute(color, $"رنگ {color}", AttributeType.Color));
            }
            return list;
        }

        private List<ProductAttribute> GetDateTimeAttributes() => new()
        {
            NewAttribute("تاریخ تولید", "تاریخ تولید محصول", AttributeType.Date),
            NewAttribute("زمان تحویل", "زمان تخمینی تحویل", AttributeType.Time),
            NewAttribute("مهلت فروش ویژه", "پایان تخفیف تا این تاریخ و زمان", AttributeType.DateTime),
        };

        private List<ProductAttribute> GetFileAttributes() => new()
        {
            NewAttribute("تصویر شاخص", "تصویر اصلی محصول", AttributeType.Image),
            NewAttribute("کاتالوگ", "فایل PDF کاتالوگ محصول", AttributeType.File),
            NewAttribute("ویدیو معرفی", "لینک به ویدیوی یوتیوب یا آپارات", AttributeType.VideoUrl),
        };

        private List<ProductAttribute> GetSelectionAttributes() => new()
        {
            NewAttribute("برند", "برند محصول از میان گزینه‌های موجود", AttributeType.Dropdown),
            NewAttribute("ویژگی‌ها", "ویژگی‌های قابل انتخاب چندتایی", AttributeType.MultiSelect),
            NewAttribute("سایز M", "سایز متوسط", AttributeType.Size),
        };

        private List<ProductAttribute> GetOtherAttributes() => new()
        {
            NewAttribute("ایمیل پشتیبانی", "آدرس ایمیل برای ارتباط با پشتیبانی", AttributeType.Email),
            NewAttribute("شماره تماس", "شماره تلفن فروشنده", AttributeType.PhoneNumber),
            NewAttribute("لینک صفحه محصول", "آدرس اینترنتی برای مشاهده محصول", AttributeType.Url),
            NewAttribute("ویژگی‌های فنی", "اطلاعات ساخت‌یافته در قالب JSON", AttributeType.Json),
            NewAttribute("محل انبار", "موقعیت مکانی کالا", AttributeType.Location),
            NewAttribute("بارکد", "کد شناسایی محصول", AttributeType.Barcode),
            NewAttribute("پیکربندی خاص", "ساختار داده‌ای سفارشی برای ویژگی خاص", AttributeType.CustomObject),
            NewAttribute("سایز", "سایز های XS , S , M ,L ,XL ,XXL", AttributeType.Size),
            NewAttribute("رنگ", "تمامی رنگ ها", AttributeType.Color),
        };

        private ProductAttribute NewAttribute(string name, string description, AttributeType type)
        {
            return new ProductAttribute
            {
                Name = name,
                Description = description,
                Type = type,
                IsDeleted = false
            };
        }
    }
}
