namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Settlement;

public class ResolutionSourceConfiguration : IEntityTypeConfiguration<ResolutionSource>
{
    public void Configure(EntityTypeBuilder<ResolutionSource> builder)
    {
        builder.ToTable("resolution_sources");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.Property(e => e.SourceType).HasConversion<string>().HasMaxLength(30);
        builder.Property(e => e.Config).HasMaxLength(4000);
    }
}
