using Entities.Excersies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Excersies
{
    public class ExerciseStepConfiguration : IEntityTypeConfiguration<ExerciseStep>
    {
        public void Configure(EntityTypeBuilder<ExerciseStep> builder)
        {
            builder.ToTable(nameof(ExerciseStep), nameof(Exercise));
            builder.HasKey(t => new { t.StepId, t.ExerciseId });
            builder.HasOne(t => t.Step).WithMany(t => t.ExerciseSteps).HasForeignKey(t => t.StepId);
            builder.HasOne(t => t.Exercise).WithMany(t => t.ExerciseSteps).HasForeignKey(t => t.ExerciseId);

        }
    }
}
