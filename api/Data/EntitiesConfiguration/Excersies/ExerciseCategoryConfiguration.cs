using Entities.Excersies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Excersies
{
    public class ExerciseCategoryConfiguration : IEntityTypeConfiguration<ExerciseCategory>
    {
        public void Configure(EntityTypeBuilder<ExerciseCategory> builder)
        {
            builder.ToTable(nameof(ExerciseCategory), nameof(Exercise));
            builder.HasMany(t => t.Excersies).WithOne(t => t.ExerciseCategory).HasForeignKey(t => t.ExerciseCategoryId);
            builder.HasQueryFilter(z => !z.IsDeleted);
        }
    }
}
