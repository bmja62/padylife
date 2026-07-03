using Entities.Questions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.Questions
{
    public class QuestionOptionConfiguration : IEntityTypeConfiguration<QuestionOption>
    {
        public void Configure(EntityTypeBuilder<QuestionOption> builder)
        {
            builder.ToTable(nameof(QuestionOption), nameof(Question));
            builder.HasOne(t => t.Question).WithMany(t => t.QuestionOptions).HasForeignKey(t => t.QuestionId);
            builder.HasQueryFilter(z => !z.IsDeleted);

            //// اعمال OrderBy برای تمام کوئری‌های QuestionOption
            //builder
            //    .Metadata.SetAnnotation("QueryFilterOrderBy",
            //        (IQueryable<QuestionOption> q) => q.OrderBy(x => x.Priority));
        }
    }
}
