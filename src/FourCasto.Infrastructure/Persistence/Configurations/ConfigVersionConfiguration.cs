namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.CountryRules;

public class ConfigVersionConfiguration : IEntityTypeConfiguration<ConfigVersion>
{
    public void Configure(EntityTypeBuilder<ConfigVersion> builder)
    {
        builder.ToTable("config_versions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.Property(e => e.CreatedBy).HasMaxLength(200);
        builder.HasIndex(e => new { e.FourCastoWlId, e.VersionNumber }).IsUnique();
    }
}
