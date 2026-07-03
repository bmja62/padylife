// Ignore Spelling: Instagram Birthdate

using System.ComponentModel.DataAnnotations;

namespace Entities.Users;

public enum MaritalStatus
{
    [Display(Name = "مجرد")]
    Single = 1,

    [Display(Name = "متاهل")]
    Married = 2
}
