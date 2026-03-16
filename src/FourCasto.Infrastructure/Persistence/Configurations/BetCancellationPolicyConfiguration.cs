namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.CountryRules;

public class BetCancellationPolicyConfiguration : IEntityTypeConfiguration<BetCancellationPolicy>
{
    public void Configure(EntityTypeBuilder<BetCancellationPolicy> builder)
    {
        builder.ToTable("bet_cancellation_policies");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.CountryCode).HasMaxLength(3);
        builder.Property(e => e.PenaltyPercent).HasColumnType("decimal(5,2)");
        builder.HasIndex(e => e.FourCastoWlId);
        builder.HasOne(e => e.ConfigVersion).WithMany().HasForeignKey(e => e.ConfigVersionId);
    }
}
