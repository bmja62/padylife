using Entities.StepOprions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.StepOptions
{
    public class TextStepOptionConfiguration : IEntityTypeConfiguration<TextStepOption>
    {
        public void Configure(EntityTypeBuilder<TextStepOption> builder)
        {
            builder.Property(x => x.Content)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(x => x.IsHtml);
            builder.Property(x => x.TextFormat).HasMaxLength(50);
        }
    }
}
