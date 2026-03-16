namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Settlement;

public class BetCancellationRequestConfiguration : IEntityTypeConfiguration<BetCancellationRequest>
{
    public void Configure(EntityTypeBuilder<BetCancellationRequest> builder)
    {
        builder.ToTable("bet_cancellation_requests");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.IdempotencyKey).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(e => e.IdempotencyKey).IsUnique();
        builder.HasIndex(e => e.BetId);
    }
}
