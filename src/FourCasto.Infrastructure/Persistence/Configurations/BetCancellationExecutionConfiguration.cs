namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Settlement;

public class BetCancellationExecutionConfiguration : IEntityTypeConfiguration<BetCancellationExecution>
{
    public void Configure(EntityTypeBuilder<BetCancellationExecution> builder)
    {
        builder.ToTable("bet_cancellation_executions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.PenaltyPercent).HasColumnType("decimal(5,2)");
        builder.Property(e => e.PenaltyAmount).HasColumnType("decimal(18,2)");
        builder.Property(e => e.AmountReturned).HasColumnType("decimal(18,2)");
        builder.Property(e => e.HoldReleaseAmount).HasColumnType("decimal(18,2)");
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.ErrorMessage).HasMaxLength(2000);
        builder.HasIndex(e => e.BetId);
        builder.HasOne(e => e.CancellationRequest).WithMany().HasForeignKey(e => e.CancellationRequestId);
    }
}
