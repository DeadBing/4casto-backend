namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.CountryRules;

public class RulePriorityPolicyConfiguration : IEntityTypeConfiguration<RulePriorityPolicy>
{
    public void Configure(EntityTypeBuilder<RulePriorityPolicy> builder)
    {
        builder.ToTable("rule_priority_policies");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.RuleType).HasMaxLength(50).IsRequired();
        builder.Property(e => e.PriorityOrder).HasMaxLength(1000).IsRequired();
        builder.HasOne(e => e.ConfigVersion).WithMany().HasForeignKey(e => e.ConfigVersionId);
    }
}
