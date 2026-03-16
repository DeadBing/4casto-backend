namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Settlement;

public class EvidenceRecordConfiguration : IEntityTypeConfiguration<EvidenceRecord>
{
    public void Configure(EntityTypeBuilder<EvidenceRecord> builder)
    {
        builder.ToTable("evidence_records");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.RefType).HasMaxLength(50).IsRequired();
        builder.Property(e => e.EvidenceType).HasMaxLength(50).IsRequired();
        builder.HasIndex(e => new { e.RefType, e.RefId });
    }
}
