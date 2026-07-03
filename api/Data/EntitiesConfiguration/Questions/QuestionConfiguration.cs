using Entities.Questions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Questions
{
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable(nameof(Question), nameof(Question));
            builder.HasOne(t => t.QuestionCategory).WithMany(t => t.Questions).HasForeignKey(t => t.QuestionCategoryId);
            builder.HasMany(t => t.QuestionOptions).WithOne(t => t.Question).HasForeignKey(t => t.QuestionId);
            builder.HasQueryFilter(z => !z.IsDeleted);
            builder.HasOne(t => t.CreatedByUser).WithMany().HasForeignKey(t => t.CreatedByUserId);
        }
    }
}
