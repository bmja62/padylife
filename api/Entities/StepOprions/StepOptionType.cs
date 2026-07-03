using System.ComponentModel.DataAnnotations;

namespace Entities.StepOprions
{
    public enum StepOptionType
    {
        [Display(Name = "چند گزینه‌ای")]
        MultipleChoice,

        [Display(Name = "ویدیو")]
        Video,

        [Display(Name = "تکلیف")]
        Task,

        [Display(Name = "تعاملی")]
        Action,

        [Display(Name = "متن")]
        Text,

        [Display(Name = "تصویر")]
        Image
    }

}
