namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Markets;

public class BetPayoutSnapshotConfiguration : IEntityTypeConfiguration<BetPayoutSnapshot>
{
    public void Configure(EntityTypeBuilder<BetPayoutSnapshot> builder)
    {
        builder.ToTable("bet_payout_snapshots");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.CountryCode).HasMaxLength(3);
        builder.Property(e => e.BetDirection).HasConversion<string>().HasMaxLength(10);
        builder.Property(e => e.StakeAmount).HasColumnType("decimal(18,2)");
        builder.Property(e => e.BasePayoutPercent).HasColumnType("decimal(5,2)");
        builder.Property(e => e.MarketPriceAtEntry).HasColumnType("decimal(18,8)");
        builder.Property(e => e.ProgressPercentAtEntry).HasColumnType("decimal(5,2)");
        builder.Property(e => e.ProgressDirectionAtEntry).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.ProgressAdjustmentPercent).HasColumnType("decimal(5,2)");
        builder.Property(e => e.ProgressAdjustmentReason).HasMaxLength(500);
        builder.Property(e => e.FinalPayoutPercentApplied).HasColumnType("decimal(5,2)");
        builder.Property(e => e.PotentialProfit).HasColumnType("decimal(18,2)");
        builder.Property(e => e.TotalReturn).HasColumnType("decimal(18,2)");
        builder.Property(e => e.PolicySourceType).HasConversion<string>().HasMaxLength(30);
        builder.HasIndex(e => e.BetId).IsUnique();
    }
}
