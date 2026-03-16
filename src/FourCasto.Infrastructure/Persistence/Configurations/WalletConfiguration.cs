namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Accounts;

public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("wallets");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.CurrencyCode).HasMaxLength(10).IsRequired();
        builder.HasIndex(e => new { e.FourCastoWlId, e.UserId });
        builder.HasOne(e => e.Balance).WithOne(b => b.Wallet).HasForeignKey<WalletBalance>(b => b.WalletId);
    }
}
