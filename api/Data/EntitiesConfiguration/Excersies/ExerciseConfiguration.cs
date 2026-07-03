using Entities.Excersies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Excersies
{
    public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
    {
        public void Configure(EntityTypeBuilder<Exercise> builder)
        {
            builder.ToTable(nameof(Exercise), nameof(Exercise));
            builder.HasMany(t => t.ExerciseSteps).WithOne(t => t.Exercise).HasForeignKey(t => t.ExerciseId);
            builder.HasOne(t => t.ExerciseCategory).WithMany(t => t.Excersies).HasForeignKey(t => t.ExerciseCategoryId);
            builder.HasQueryFilter(z => !z.IsDeleted);
            builder.HasOne(t => t.CreatedByUser).WithMany().HasForeignKey(t => t.CreatedByUserId);
        }
    }
}
