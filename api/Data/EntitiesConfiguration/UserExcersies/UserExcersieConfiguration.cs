using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.UserExcersies
{
    internal class UserExcersieConfiguration : IEntityTypeConfiguration<UserExercise>
    {
        public void Configure(EntityTypeBuilder<UserExercise> builder)
        {
            builder.ToTable(nameof(UserExercise), nameof(User));

            builder.HasKey(t => t.Id);

            builder.HasOne(t => t.User).WithMany(t => t.UserExercises).HasForeignKey(t => t.UserId);
            builder.HasOne(t => t.Exercise).WithMany(t => t.UserExercises).HasForeignKey(t => t.ExerciseId);
        }
    }
}
