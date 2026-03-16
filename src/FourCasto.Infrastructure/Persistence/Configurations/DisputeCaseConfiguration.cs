namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Settlement;

public class DisputeCaseConfiguration : IEntityTypeConfiguration<DisputeCase>
{
    public void Configure(EntityTypeBuilder<DisputeCase> builder)
    {
        builder.ToTable("dispute_cases");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.RefType).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.Description).HasMaxLength(2000).IsRequired();
        builder.HasIndex(e => new { e.RefType, e.RefId });
        builder.HasIndex(e => e.Status);
    }
}
