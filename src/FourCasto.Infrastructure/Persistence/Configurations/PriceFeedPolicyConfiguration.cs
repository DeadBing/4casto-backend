namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Pricing;

public class PriceFeedPolicyConfiguration : IEntityTypeConfiguration<PriceFeedPolicy>
{
    public void Configure(EntityTypeBuilder<PriceFeedPolicy> builder)
    {
        builder.ToTable("price_feed_policies");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.HasIndex(e => new { e.FourCastoWlId, e.SubjectId });
        builder.HasOne(e => e.QuoteSource).WithMany().HasForeignKey(e => e.QuoteSourceId);
    }
}
