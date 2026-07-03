using Entities.Medals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Medals
{
    public class MedalConfiguration : IEntityTypeConfiguration<Medal>
    {
        public void Configure(EntityTypeBuilder<Medal> builder)
        {
            builder.ToTable(nameof(Medal), nameof(Medal));
            builder.HasMany(t => t.Conditions)
                .WithOne(t => t.Medal)
                .HasForeignKey(t => t.MedalId);

            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }

    public class MedalConditionConfiguration : IEntityTypeConfiguration<MedalCondition>
    {
        public void Configure(EntityTypeBuilder<MedalCondition> builder)
        {
            builder.ToTable(nameof(MedalCondition), nameof(Medal));
            builder.HasOne(t => t.Medal)
                .WithMany(t => t.Conditions)
                .HasForeignKey(t => t.MedalId);
            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }

    public class UserMedalConfiguration : IEntityTypeConfiguration<UserMedal>
    {
        public void Configure(EntityTypeBuilder<UserMedal> builder)
        {
            builder.ToTable(nameof(UserMedal), nameof(Medal));

            builder.HasOne(t => t.Medal)
                .WithMany(t => t.UserMedals)
                .HasForeignKey(t => t.MedalId);

            builder.HasOne(t => t.User)
                .WithMany(t => t.UserMedals)
                .HasForeignKey(t => t.UserId);
            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}
