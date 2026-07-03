using Entities.Plans;
using Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Plans
{
    public class UserPlanAnswerConfiguration : IEntityTypeConfiguration<UserPlanAnswer>
    {
        public void Configure(EntityTypeBuilder<UserPlanAnswer> builder)
        {
            //Name And Schema
            builder.ToTable(nameof(UserPlanAnswer), nameof(Plan));

            builder.HasOne(t => t.UserPlan).WithMany(t => t.Answers).HasForeignKey(t => t.UserPlanId);
            builder.HasOne(t => t.PlanQuestion).WithMany(t => t.UserPlanQuestionAnswers).HasForeignKey(t => t.PlanQuestionId);
            builder.HasOne(t => t.SelectedQuestionOption).WithMany(t => t.SelectedOptionAnswers).HasForeignKey(t => t.SelectedQuestionOptionId);

            //Filters
            builder.HasQueryFilter(t => !t.IsDeleted);

        }
    }
}
