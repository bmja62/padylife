using Entities.Excersies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntitiesConfiguration.StepOptions
{
    public class OptionChoiceConfiguration : IEntityTypeConfiguration<OptionChoice>
    {
        public void Configure(EntityTypeBuilder<OptionChoice> builder)
        {
            builder.ToTable(nameof(OptionChoice), nameof(Step));

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Text)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.IsCorrect)
                .IsRequired();

            builder.Property(x => x.Order)
                .IsRequired();

            builder
                .HasOne(x => x.StepOption)
                .WithMany(x => x.Choices)
                .HasForeignKey(x => x.StepOptionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
