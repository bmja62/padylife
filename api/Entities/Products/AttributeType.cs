namespace Entities.Products
{
    using System.ComponentModel.DataAnnotations;

    public enum AttributeType
    {
        [Display(Name = "متن ساده")]
        Text,

        [Display(Name = "عدد صحیح")]
        Number,

        [Display(Name = "عدد اعشاری")]
        Decimal,

        [Display(Name = "درست یا نادرست")]
        Boolean,

        [Display(Name = "رنگ")]
        Color,

        [Display(Name = "سایز")]
        Size,

        [Display(Name = "تاریخ")]
        Date,

        [Display(Name = "زمان")]
        Time,

        [Display(Name = "تاریخ و زمان")]
        DateTime,

        [Display(Name = "تصویر")]
        Image,

        [Display(Name = "فایل")]
        File,

        [Display(Name = "فهرست کشویی")]
        Dropdown,

        [Display(Name = "انتخاب چندتایی")]
        MultiSelect,

        [Display(Name = "برچسب‌ها")]
        Tags,

        [Display(Name = "بازه عددی")]
        Range,

        [Display(Name = "متن با فرمت")]
        RichText,

        [Display(Name = "ایمیل")]
        Email,

        [Display(Name = "شماره تلفن")]
        PhoneNumber,

        [Display(Name = "آدرس اینترنتی")]
        Url,

        [Display(Name = "لینک ویدیو")]
        VideoUrl,

        [Display(Name = "داده ساخت‌یافته (JSON)")]
        Json,

        [Display(Name = "موقعیت مکانی")]
        Location,

        [Display(Name = "بارکد یا کد کالا")]
        Barcode,

        [Display(Name = "امتیاز عددی")]
        Rating,

        [Display(Name = "مقدار پولی")]
        Currency,

        [Display(Name = "درصد")]
        Percentage,

        [Display(Name = "مدت زمان")]
        TimeDuration,

        [Display(Name = "شیء دلخواه")]
        CustomObject
    }

}
