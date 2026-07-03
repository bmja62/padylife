using System.ComponentModel.DataAnnotations;

namespace Entities.Excersies
{
    public enum ExerciseType
    {
        [Display(Name = "دیداری")]
        Visual,
        [Display(Name = "شنیداری")]
        Auditory
    }
}
