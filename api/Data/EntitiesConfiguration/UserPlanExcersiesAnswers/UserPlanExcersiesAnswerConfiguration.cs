using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.UserPlanExcersiesAnswers
{
    public class UserPlanExcersiesAnswerConfiguration : IEntityTypeConfiguration<UserPlanExcersiesAnswer>
    {
        public void Configure(EntityTypeBuilder<UserPlanExcersiesAnswer> builder)
        {
            builder.ToTable(nameof(UserPlanExcersiesAnswer), nameof(UserPlanExcersiesAnswer));

            builder.HasKey(x => x.Id);

            builder.HasAlternateKey(x => new { x.UserPlanId, x.StepId, x.ExcersieId });

            builder
           .HasMany(x => x.SelectedChoices)
           .WithOne(x => x.Answer)
           .HasForeignKey(x => x.UserPlanExcersiesAnswerId);

            builder.HasOne(t => t.Exercise).WithMany(t => t.UserPlanExcersiesAnswers).HasForeignKey(t => t.ExcersieId);
            builder.HasOne(t => t.Step).WithMany(t => t.UserPlanExcersiesAnswers).HasForeignKey(t => t.StepId);
            builder.HasOne(t => t.SelectedStepOption).WithMany(t => t.UserPlanExcersiesAnswers).HasForeignKey(t => t.SelectedStepOptionId);

        }
    }
    public class UserSelectedChoiceConfiguration : IEntityTypeConfiguration<UserSelectedChoice>
    {
        public void Configure(EntityTypeBuilder<UserSelectedChoice> builder)
        {
            builder.ToTable(nameof(UserSelectedChoice), nameof(UserPlanExcersiesAnswer));

            builder.HasKey(t => new { t.OptionChoiceId, t.UserPlanExcersiesAnswerId });

            builder.HasOne(x => x.Choice)
                   .WithMany(x => x.UserSelectedChoices)
                   .HasForeignKey(x => x.OptionChoiceId);

            builder.HasOne(x => x.Answer)
                   .WithMany(x => x.SelectedChoices)
                   .HasForeignKey(x => x.UserPlanExcersiesAnswerId);
        }
    }
}
