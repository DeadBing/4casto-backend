namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Markets;

public class SignalOutcomeConfiguration : IEntityTypeConfiguration<SignalOutcome>
{
    public void Configure(EntityTypeBuilder<SignalOutcome> builder)
    {
        builder.ToTable("signal_outcomes");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.OutcomeType).HasConversion<string>().HasMaxLength(30);
        builder.Property(e => e.ResolvedPrice).HasColumnType("decimal(18,8)");
        builder.Property(e => e.Notes).HasMaxLength(1000);
        builder.HasIndex(e => e.SignalId).IsUnique();
    }
}
