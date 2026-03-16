namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Accounts;

public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable("payment_transactions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.TransactionType).HasMaxLength(20);
        builder.Property(e => e.Amount).HasColumnType("decimal(18,2)");
        builder.Property(e => e.CurrencyCode).HasMaxLength(10);
        builder.Property(e => e.Status).HasMaxLength(20);
        builder.Property(e => e.ExternalTransactionId).HasMaxLength(200);
        builder.Property(e => e.IdempotencyKey).HasMaxLength(100);
        builder.HasIndex(e => e.IdempotencyKey).IsUnique();
        builder.HasIndex(e => new { e.FourCastoWlId, e.UserId });
    }
}
