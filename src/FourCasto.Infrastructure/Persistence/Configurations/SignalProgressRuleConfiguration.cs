namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.CountryRules;

public class SignalProgressRuleConfiguration : IEntityTypeConfiguration<SignalProgressRule>
{
    public void Configure(EntityTypeBuilder<SignalProgressRule> builder)
    {
        builder.ToTable("signal_progress_rules");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.ProgressFrom).HasColumnType("decimal(5,2)");
        builder.Property(e => e.ProgressTo).HasColumnType("decimal(5,2)");
        builder.Property(e => e.ProgressDirection).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.PayoutAdjustmentPercent).HasColumnType("decimal(5,2)");
        builder.HasOne(e => e.ConfigVersion).WithMany().HasForeignKey(e => e.ConfigVersionId);
    }
}
