namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Accounts;

public class LedgerEntryConfiguration : IEntityTypeConfiguration<LedgerEntry>
{
    public void Configure(EntityTypeBuilder<LedgerEntry> builder)
    {
        builder.ToTable("ledger_entries");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.AccountType).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.BalanceType).HasConversion<string>().HasMaxLength(10);
        builder.Property(e => e.EntryType).HasConversion<string>().HasMaxLength(30);
        builder.Property(e => e.Amount).HasColumnType("decimal(18,2)");
        builder.Property(e => e.RefType).HasMaxLength(50);
        builder.Property(e => e.BalanceTotalAfter).HasColumnType("decimal(18,2)");
        builder.Property(e => e.BalanceAvailableAfter).HasColumnType("decimal(18,2)");
        builder.Property(e => e.BalanceLockedAfter).HasColumnType("decimal(18,2)");
        builder.HasIndex(e => new { e.FourCastoWlId, e.AccountType, e.AccountId });
        builder.HasIndex(e => new { e.RefType, e.RefId });
        builder.HasIndex(e => e.CreatedAt);
    }
}
