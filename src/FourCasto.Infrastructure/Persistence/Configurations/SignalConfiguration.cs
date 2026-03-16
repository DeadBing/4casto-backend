namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Markets;

public class SignalConfiguration : IEntityTypeConfiguration<Signal>
{
    public void Configure(EntityTypeBuilder<Signal> builder)
    {
        builder.ToTable("signals");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.SignalType).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.SignalDirection).HasConversion<string>().HasMaxLength(10);
        builder.Property(e => e.EntryPrice).HasColumnType("decimal(18,8)");
        builder.Property(e => e.TargetPrice).HasColumnType("decimal(18,8)");
        builder.Property(e => e.StopLossPrice).HasColumnType("decimal(18,8)");
        builder.Property(e => e.MaxBettingProgressPercent).HasColumnType("decimal(5,2)");
        builder.Property(e => e.BasePayoutPercentAgree).HasColumnType("decimal(5,2)");
        builder.Property(e => e.BasePayoutPercentDisagree).HasColumnType("decimal(5,2)");
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(e => new { e.FourCastoWlId, e.Status });
        builder.HasOne(e => e.Subject).WithMany().HasForeignKey(e => e.SubjectId);
        builder.HasOne(e => e.Outcome).WithOne(o => o.Signal).HasForeignKey<SignalOutcome>(o => o.SignalId);
    }
}
