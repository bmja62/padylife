// Ignore Spelling: Instagram Birthdate

using System.ComponentModel.DataAnnotations;

namespace Entities.Users;

public enum GenderType
{
    [Display(Name = "مرد")]
    Male = 1,

    [Display(Name = "زن")]
    Female = 2
}
