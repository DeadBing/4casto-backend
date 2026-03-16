namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Pricing;

public class QuoteSourceConfiguration : IEntityTypeConfiguration<QuoteSource>
{
    public void Configure(EntityTypeBuilder<QuoteSource> builder)
    {
        builder.ToTable("quote_sources");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.Property(e => e.SourceType).HasMaxLength(50).IsRequired();
    }
}
