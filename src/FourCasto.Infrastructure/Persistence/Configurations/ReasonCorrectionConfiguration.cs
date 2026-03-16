namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Settlement;

public class ReasonCorrectionConfiguration : IEntityTypeConfiguration<ReasonCorrection>
{
    public void Configure(EntityTypeBuilder<ReasonCorrection> builder)
    {
        builder.ToTable("reason_corrections");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.RefType).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Reason).HasMaxLength(1000).IsRequired();
        builder.Property(e => e.CorrectedBy).HasMaxLength(200).IsRequired();
        builder.HasIndex(e => new { e.RefType, e.RefId });
    }
}
