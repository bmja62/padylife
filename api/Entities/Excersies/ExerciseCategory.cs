using Entities.Common;

namespace Entities.Excersies
{
    //دسته بندی تمرین
    public class ExerciseCategory : BaseEntity<long>
    {
        //Props
        public string Name { get; set; }
        //Navigations
        public ICollection<Exercise> Excersies { get; set; }
    }
}
