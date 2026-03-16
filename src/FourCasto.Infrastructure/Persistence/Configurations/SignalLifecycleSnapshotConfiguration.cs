namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Markets;

public class SignalLifecycleSnapshotConfiguration : IEntityTypeConfiguration<SignalLifecycleSnapshot>
{
    public void Configure(EntityTypeBuilder<SignalLifecycleSnapshot> builder)
    {
        builder.ToTable("signal_lifecycle_snapshots");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.CurrentMarketPrice).HasColumnType("decimal(18,8)");
        builder.Property(e => e.ProgressPercent).HasColumnType("decimal(5,2)");
        builder.Property(e => e.ProgressDirection).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(e => new { e.SignalId, e.SnapshotAt });
    }
}
