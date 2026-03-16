namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Fraud;

public class FraudRuleConfiguration : IEntityTypeConfiguration<FraudRule>
{
    public void Configure(EntityTypeBuilder<FraudRule> builder)
    {
        builder.ToTable("fraud_rules");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.RuleName).HasMaxLength(200).IsRequired();
        builder.Property(e => e.RuleType).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Config).HasMaxLength(4000);
        builder.HasIndex(e => e.FourCastoWlId);
    }
}
