namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Accounts;

public class WalletTransferConfiguration : IEntityTypeConfiguration<WalletTransfer>
{
    public void Configure(EntityTypeBuilder<WalletTransfer> builder)
    {
        builder.ToTable("wallet_transfers");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Amount).HasColumnType("decimal(18,2)");
        builder.Property(e => e.CurrencyCode).HasMaxLength(10);
        builder.Property(e => e.Direction).HasConversion<string>().HasMaxLength(30);
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.IdempotencyKey).HasMaxLength(100).IsRequired();
        builder.HasIndex(e => e.IdempotencyKey).IsUnique();
        builder.HasIndex(e => new { e.FourCastoWlId, e.UserId });
    }
}
