namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Markets;

public class BetEntryPriceContextConfiguration : IEntityTypeConfiguration<BetEntryPriceContext>
{
    public void Configure(EntityTypeBuilder<BetEntryPriceContext> builder)
    {
        builder.ToTable("bet_entry_price_contexts");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.EntryPrice).HasColumnType("decimal(18,8)");
        builder.Property(e => e.CurrentMarketPrice).HasColumnType("decimal(18,8)");
        builder.Property(e => e.TargetPrice).HasColumnType("decimal(18,8)");
        builder.Property(e => e.StopLossPrice).HasColumnType("decimal(18,8)");
        builder.Property(e => e.ProgressPercent).HasColumnType("decimal(5,2)");
        builder.Property(e => e.ProgressDirection).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(e => e.BetId).IsUnique();
    }
}
