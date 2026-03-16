namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.CountryRules;

public class PolicyResolutionLogConfiguration : IEntityTypeConfiguration<PolicyResolutionLog>
{
    public void Configure(EntityTypeBuilder<PolicyResolutionLog> builder)
    {
        builder.ToTable("policy_resolution_logs");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.RuleType).HasMaxLength(50).IsRequired();
        builder.Property(e => e.FallbackChain).HasMaxLength(2000);
        builder.HasIndex(e => e.BetId);
    }
}
