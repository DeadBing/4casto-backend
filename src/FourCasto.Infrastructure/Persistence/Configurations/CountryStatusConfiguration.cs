namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.CountryRules;

public class CountryStatusConfiguration : IEntityTypeConfiguration<CountryStatus>
{
    public void Configure(EntityTypeBuilder<CountryStatus> builder)
    {
        builder.ToTable("country_statuses");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Name).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(e => new { e.FourCastoWlId, e.Name }).IsUnique();
    }
}
