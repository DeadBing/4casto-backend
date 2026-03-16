namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Accounts;

public class FundingSourceConfiguration : IEntityTypeConfiguration<FundingSource>
{
    public void Configure(EntityTypeBuilder<FundingSource> builder)
    {
        builder.ToTable("funding_sources");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.SourceType).HasMaxLength(50);
        builder.Property(e => e.DisplayName).HasMaxLength(200);
        builder.Property(e => e.MaskedDetails).HasMaxLength(200);
        builder.HasIndex(e => new { e.FourCastoWlId, e.UserId });
    }
}
