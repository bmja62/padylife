using Entities.Excersies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.ExerciseLinkeds
{
    public class ExerciseLinkedConfiguration : IEntityTypeConfiguration<ExerciseLinked>
    {
        public void Configure(EntityTypeBuilder<ExerciseLinked> builder)
        {
            builder.ToTable(nameof(ExerciseLinked), nameof(ExerciseLinked));

            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(t => new { t.ExerciseId, t.QuestionLinkedId });

            builder.HasOne(t => t.Exercise).WithMany(t => t.ExerciseLinks).HasForeignKey(t => t.ExerciseId);
            builder.HasOne(t => t.QuestionLinked).WithMany(t => t.ExerciseLinks).HasForeignKey(t => t.QuestionLinkedId);

        }
    }
}
