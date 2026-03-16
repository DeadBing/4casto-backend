namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.CountryRules;

public class SignalAvailabilityRuleConfiguration : IEntityTypeConfiguration<SignalAvailabilityRule>
{
    public void Configure(EntityTypeBuilder<SignalAvailabilityRule> builder)
    {
        builder.ToTable("signal_availability_rules");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.MaxProgressPercent).HasColumnType("decimal(5,2)");
        builder.HasOne(e => e.ConfigVersion).WithMany().HasForeignKey(e => e.ConfigVersionId);
    }
}
