namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Accounts;

public class WalletBalanceConfiguration : IEntityTypeConfiguration<WalletBalance>
{
    public void Configure(EntityTypeBuilder<WalletBalance> builder)
    {
        builder.ToTable("wallet_balances");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.TotalBalance).HasColumnType("decimal(18,2)");
        builder.Property(e => e.AvailableBalance).HasColumnType("decimal(18,2)");
        builder.Property(e => e.LockedBalance).HasColumnType("decimal(18,2)");
        builder.Property(e => e.WithdrawableBalance).HasColumnType("decimal(18,2)");
        builder.Property(e => e.RowVersion).IsConcurrencyToken();
        builder.HasIndex(e => e.WalletId).IsUnique();
    }
}
