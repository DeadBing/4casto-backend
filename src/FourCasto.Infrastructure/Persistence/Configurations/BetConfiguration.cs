namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Markets;

public class BetConfiguration : IEntityTypeConfiguration<Bet>
{
    public void Configure(EntityTypeBuilder<Bet> builder)
    {
        builder.ToTable("bets");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Direction).HasConversion<string>().HasMaxLength(10);
        builder.Property(e => e.StakeAmount).HasColumnType("decimal(18,2)");
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(e => new { e.FourCastoWlId, e.UserId, e.Status });
        builder.HasIndex(e => e.MarketId);
        builder.HasOne(e => e.Market).WithMany().HasForeignKey(e => e.MarketId);
        builder.HasOne(e => e.PayoutSnapshot).WithOne(s => s.Bet).HasForeignKey<BetPayoutSnapshot>(s => s.BetId);
        builder.HasOne(e => e.EntryPriceContext).WithOne(c => c.Bet).HasForeignKey<BetEntryPriceContext>(c => c.BetId);
    }
}
