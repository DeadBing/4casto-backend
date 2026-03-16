namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.CountryRules;

public class CountryStatusQualificationRuleConfiguration : IEntityTypeConfiguration<CountryStatusQualificationRule>
{
    public void Configure(EntityTypeBuilder<CountryStatusQualificationRule> builder)
    {
        builder.ToTable("country_status_qualification_rules");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.CountryCode).HasMaxLength(3).IsRequired();
        builder.Property(e => e.MetricType).HasConversion<string>().HasMaxLength(30);
        builder.Property(e => e.MinValue).HasColumnType("decimal(18,2)");
        builder.Property(e => e.MaxValue).HasColumnType("decimal(18,2)");
        builder.HasOne(e => e.CountryStatus).WithMany().HasForeignKey(e => e.CountryStatusId);
        builder.HasOne(e => e.ConfigVersion).WithMany().HasForeignKey(e => e.ConfigVersionId);
    }
}
