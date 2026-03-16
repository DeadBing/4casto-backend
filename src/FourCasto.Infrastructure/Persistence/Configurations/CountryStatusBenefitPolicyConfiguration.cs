namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.CountryRules;

public class CountryStatusBenefitPolicyConfiguration : IEntityTypeConfiguration<CountryStatusBenefitPolicy>
{
    public void Configure(EntityTypeBuilder<CountryStatusBenefitPolicy> builder)
    {
        builder.ToTable("country_status_benefit_policies");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.CountryCode).HasMaxLength(3);
        builder.Property(e => e.BenefitType).HasConversion<string>().HasMaxLength(30);
        builder.Property(e => e.BenefitValue).HasColumnType("decimal(18,2)");
        builder.HasOne(e => e.CountryStatus).WithMany().HasForeignKey(e => e.CountryStatusId);
        builder.HasOne(e => e.ConfigVersion).WithMany().HasForeignKey(e => e.ConfigVersionId);
    }
}
