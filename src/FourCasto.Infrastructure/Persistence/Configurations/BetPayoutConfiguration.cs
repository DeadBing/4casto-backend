namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Settlement;

public class BetPayoutConfiguration : IEntityTypeConfiguration<BetPayout>
{
    public void Configure(EntityTypeBuilder<BetPayout> builder)
    {
        builder.ToTable("bet_payouts");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.PayoutAmount).HasColumnType("decimal(18,2)");
        builder.Property(e => e.PnlAmount).HasColumnType("decimal(18,2)");
        builder.Property(e => e.PayoutType).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(e => e.BetId);
        builder.HasOne(e => e.Settlement).WithMany().HasForeignKey(e => e.SettlementId);
    }
}
