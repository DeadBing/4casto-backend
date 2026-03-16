namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.CountryRules;

public class PayoutPolicyRuleConfiguration : IEntityTypeConfiguration<PayoutPolicyRule>
{
    public void Configure(EntityTypeBuilder<PayoutPolicyRule> builder)
    {
        builder.ToTable("payout_policy_rules");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.CountryCode).HasMaxLength(3);
        builder.Property(e => e.BetDirection).HasConversion<string>().HasMaxLength(10);
        builder.Property(e => e.PayoutPercent).HasColumnType("decimal(5,2)");
        builder.HasIndex(e => new { e.FourCastoWlId, e.Priority });
        builder.HasOne(e => e.ConfigVersion).WithMany().HasForeignKey(e => e.ConfigVersionId);
    }
}
