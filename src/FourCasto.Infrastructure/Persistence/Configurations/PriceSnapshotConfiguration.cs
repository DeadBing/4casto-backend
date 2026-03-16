namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Pricing;

public class PriceSnapshotConfiguration : IEntityTypeConfiguration<PriceSnapshot>
{
    public void Configure(EntityTypeBuilder<PriceSnapshot> builder)
    {
        builder.ToTable("price_snapshots");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Price).HasColumnType("decimal(18,8)");
        builder.HasIndex(e => new { e.SubjectId, e.SnapshotAt });
    }
}
