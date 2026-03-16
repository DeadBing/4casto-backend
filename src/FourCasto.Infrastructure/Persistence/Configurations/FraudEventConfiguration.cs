namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Fraud;

public class FraudEventConfiguration : IEntityTypeConfiguration<FraudEvent>
{
    public void Configure(EntityTypeBuilder<FraudEvent> builder)
    {
        builder.ToTable("fraud_events");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.EventType).HasConversion<string>().HasMaxLength(30);
        builder.Property(e => e.Details).HasMaxLength(4000);
        builder.HasIndex(e => new { e.FourCastoWlId, e.UserId });
        builder.HasOne(e => e.FraudRule).WithMany().HasForeignKey(e => e.FraudRuleId);
    }
}
