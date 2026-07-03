using Entities.Questions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Questions
{
    public class QuestionCategoryConfiguration : IEntityTypeConfiguration<QuestionCategory>
    {
        public void Configure(EntityTypeBuilder<QuestionCategory> builder)
        {
            builder.ToTable(nameof(QuestionCategory), nameof(Question));
            builder.HasMany(t => t.Questions).WithOne(t => t.QuestionCategory).HasForeignKey(t => t.QuestionCategoryId);
            builder.HasQueryFilter(z => !z.IsDeleted);
        }
    }
}
