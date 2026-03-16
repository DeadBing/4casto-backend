namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Pricing;

public class PriceTickConfiguration : IEntityTypeConfiguration<PriceTick>
{
    public void Configure(EntityTypeBuilder<PriceTick> builder)
    {
        builder.ToTable("price_ticks");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Price).HasColumnType("decimal(18,8)");
        builder.HasIndex(e => new { e.SubjectId, e.ReceivedAt });
    }
}
