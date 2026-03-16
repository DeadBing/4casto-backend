namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Settlement;

public class BetSettlementExecutionConfiguration : IEntityTypeConfiguration<BetSettlementExecution>
{
    public void Configure(EntityTypeBuilder<BetSettlementExecution> builder)
    {
        builder.ToTable("bet_settlement_executions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.ErrorMessage).HasMaxLength(2000);
        builder.HasIndex(e => new { e.BetId, e.SettlementId });
    }
}
