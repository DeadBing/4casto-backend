namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Settlement;

public class SettlementAdjustmentConfiguration : IEntityTypeConfiguration<SettlementAdjustment>
{
    public void Configure(EntityTypeBuilder<SettlementAdjustment> builder)
    {
        builder.ToTable("settlement_adjustments");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.OriginalPayoutAmount).HasColumnType("decimal(18,2)");
        builder.Property(e => e.AdjustedPayoutAmount).HasColumnType("decimal(18,2)");
        builder.Property(e => e.Reason).HasMaxLength(1000).IsRequired();
        builder.Property(e => e.AdjustedBy).HasMaxLength(200).IsRequired();
        builder.HasIndex(e => e.SettlementId);
        builder.HasOne(e => e.Settlement).WithMany().HasForeignKey(e => e.SettlementId);
    }
}
