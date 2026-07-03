using Entities.Plans;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Plans
{
    public class PlanQuestionConfiguration : IEntityTypeConfiguration<PlanQuestion>
    {
        public void Configure(EntityTypeBuilder<PlanQuestion> builder)
        {
            builder.ToTable(nameof(PlanQuestion), nameof(Plan));

            //Navigations
            builder.HasOne(t => t.Plan).WithMany(t => t.PlanQuestions).HasForeignKey(t => t.PlanId);
            builder.HasOne(t => t.Question).WithMany(t => t.PlanQuestions).HasForeignKey(t => t.QuestionId);

            //Filters
            builder.HasQueryFilter(t => !t.IsDeleted);
        }
    }
}
