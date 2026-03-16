namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Accounts;

public class TradingAccountConfiguration : IEntityTypeConfiguration<TradingAccount>
{
    public void Configure(EntityTypeBuilder<TradingAccount> builder)
    {
        builder.ToTable("trading_accounts");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.AccountType).HasConversion<string>().HasMaxLength(10);
        builder.Property(e => e.AccountNumber).HasMaxLength(50).IsRequired();
        builder.Property(e => e.CurrencyCode).HasMaxLength(10).IsRequired();
        builder.HasIndex(e => new { e.FourCastoWlId, e.UserId });
        builder.HasIndex(e => e.AccountNumber).IsUnique();
        builder.HasOne(e => e.Balance).WithOne(b => b.TradingAccount).HasForeignKey<TradingAccountBalance>(b => b.TradingAccountId);
    }
}
