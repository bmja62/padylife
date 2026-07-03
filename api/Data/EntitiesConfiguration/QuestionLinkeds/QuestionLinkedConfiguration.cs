using Entities.Questions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.QuestionLinkeds
{
    public class QuestionLinkedConfiguration : IEntityTypeConfiguration<QuestionLinked>
    {
        public void Configure(EntityTypeBuilder<QuestionLinked> builder)
        {
            builder.ToTable(nameof(QuestionLinked), nameof(QuestionLinked));

            builder.HasKey(x => x.Id);
            builder.HasAlternateKey(t => new { t.PlanId, t.PlanQuestionId, t.QuestionOptionId });

            builder.HasOne(t => t.Plan).WithMany(t => t.QuestionLinks).HasForeignKey(t => t.PlanId);
            builder.HasOne(t => t.PlanQuestion).WithMany(t => t.QuestionLinks).HasForeignKey(t => t.PlanQuestionId);
            builder.HasOne(t => t.QuestionOption).WithMany(t => t.QuestionLinks).HasForeignKey(t => t.QuestionOptionId);
            builder.HasOne(t => t.LinkedPlanQuestion).WithMany(t => t.PlanQuestionLinks).HasForeignKey(t => t.LinkedPlanQuestionId);
            builder.HasMany(t => t.ExerciseLinks).WithOne(t => t.QuestionLinked).HasForeignKey(t => t.QuestionLinkedId);
        }
    }
}
