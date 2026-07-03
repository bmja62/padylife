using Entities.StepOprions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.StepOptions
{
    public class MultipleChoiceStepOptionConfiguration : IEntityTypeConfiguration<MultipleChoiceStepOption>
    {
        public void Configure(EntityTypeBuilder<MultipleChoiceStepOption> builder)
        {
            builder
                .HasMany(x => x.Choices)
                .WithOne(x => x.StepOption)
                .HasForeignKey(x => x.StepOptionId);

            builder.Property(x => x.AllowMultipleSelection).IsRequired();
            builder.Property(x => x.CorrectAnswerHint).HasMaxLength(500);
        }
    }
}
