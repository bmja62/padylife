using Entities.Common;

namespace Entities.Questions
{
    // مدل دسته‌بندی سوالات
    public class QuestionCategory : BaseEntity<long>
    {
        public string Name { get; set; }

        //Navigations
        public ICollection<Question> Questions { get; set; }
    }
}
