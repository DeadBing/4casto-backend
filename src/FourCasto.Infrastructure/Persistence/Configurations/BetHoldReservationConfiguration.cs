namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Accounts;

public class BetHoldReservationConfiguration : IEntityTypeConfiguration<BetHoldReservation>
{
    public void Configure(EntityTypeBuilder<BetHoldReservation> builder)
    {
        builder.ToTable("bet_hold_reservations");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.AmountLocked).HasColumnType("decimal(18,2)");
        builder.Property(e => e.CurrencyCode).HasMaxLength(10);
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.ReleaseReason).HasConversion<string>().HasMaxLength(30);
        builder.HasIndex(e => e.BetId);
        builder.HasIndex(e => new { e.TradingAccountId, e.Status });
    }
}
