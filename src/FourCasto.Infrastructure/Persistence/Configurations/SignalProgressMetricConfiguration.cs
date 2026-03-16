namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Markets;

public class SignalProgressMetricConfiguration : IEntityTypeConfiguration<SignalProgressMetric>
{
    public void Configure(EntityTypeBuilder<SignalProgressMetric> builder)
    {
        builder.ToTable("signal_progress_metrics");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.ProgressPercent).HasColumnType("decimal(5,2)");
        builder.Property(e => e.ProgressDirection).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.MarketPrice).HasColumnType("decimal(18,8)");
        builder.HasIndex(e => new { e.SignalId, e.CalculatedAt });
    }
}
