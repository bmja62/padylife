using System.ComponentModel.DataAnnotations;

namespace Entities.Tickets
{
    public enum TicketType : byte
    {
        [Display(Name = "کارشناس")]
        Expert = 1,
        [Display(Name = "متخصص تغذیه")]
        NutritionSpecialist,
        [Display(Name = "امور‌مالی")]
        Financial,
        [Display(Name = "پیشنهادات")]
        Suggestion,
        [Display(Name = "سایر")]
        Other,
    }


}
