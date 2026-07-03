using System.ComponentModel.DataAnnotations;

namespace Entities.Tickets
{
    public enum TicketStatus : byte
    {
        [Display(Name = "در انتظار پشتیبان")]
        WaitingForSupport = 1,
        [Display(Name = "در انتظار کاربر")]
        WaitingForUser = 2,
        [Display(Name = "بسته شده")]
        Closed = 3,
    }


}
