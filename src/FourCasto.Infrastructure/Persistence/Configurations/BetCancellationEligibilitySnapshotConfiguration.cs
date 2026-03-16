namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Settlement;

public class BetCancellationEligibilitySnapshotConfiguration : IEntityTypeConfiguration<BetCancellationEligibilitySnapshot>
{
    public void Configure(EntityTypeBuilder<BetCancellationEligibilitySnapshot> builder)
    {
        builder.ToTable("bet_cancellation_eligibility_snapshots");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.CurrentMarketPrice).HasColumnType("decimal(18,8)");
        builder.Property(e => e.EntryPrice).HasColumnType("decimal(18,8)");
        builder.Property(e => e.BetDirection).HasConversion<string>().HasMaxLength(10);
        builder.Property(e => e.SignalDirection).HasConversion<string>().HasMaxLength(10);
        builder.Property(e => e.ApplicablePenaltyPercent).HasColumnType("decimal(5,2)");
        builder.Property(e => e.DenialReason).HasMaxLength(500);
        builder.HasIndex(e => e.BetId);
    }
}
