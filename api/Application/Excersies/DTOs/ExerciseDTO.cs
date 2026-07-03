namespace Application.Excersies.DTOs
{
    public class ExerciseDTO
    {
        public ExerciseDTO()
        {

        }
        public ExerciseDTO(long exerciseId, string title, string categoryName)
        {
            Id = exerciseId;
            Title = title;
            CategoryName = categoryName;
        }

        public long Id { get; internal set; }
        public string Title { get; internal set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }

        internal static ExerciseDTO Create(long exerciseId, string title, string categoryName)
        => new(exerciseId, title, categoryName);
    }
}
