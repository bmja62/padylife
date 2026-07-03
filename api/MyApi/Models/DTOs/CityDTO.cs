using Entities.Blogs;
using Entities.Discounts;
using Entities.Locations;
using System.ComponentModel.DataAnnotations;
using WebFramework.Api;

namespace PadyLife.Api.Models.DTOs
{
    public class CityDTO : BaseDto<CityDTO, City, long>
    {
        [Required]
        public long ProvinceId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CityName { get; set; }

        [Required]
        [MaxLength(100)]
        public string CityNameFa { get; set; }

        [MaxLength(10)]
        public string CityCode { get; set; }

        public bool IsActive { get; set; } = true;

    }

    public class DiscountDTO :BaseDto<DiscountDTO, Discount,long>
    {
        public string Code { get; set; }   // کد تخفیف
        public decimal DiscountAmount { get; set; }  // تخفیف به صورت عددی
        public decimal DiscountPercentage { get; set; } // تخفیف به صورت درصدی
        public DateTime? StartDate { get; set; } // تاریخ شروع تخفیف
        public DateTime? EndDate { get; set; }   // تاریخ پایان تخفیف
        public bool IsSpecial { get; set; }   // تخفیف ویژه (جشنواره)
    }
    public class BlogCategoryDTO : BaseDto<BlogCategoryDTO, BlogCategory, long>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}
