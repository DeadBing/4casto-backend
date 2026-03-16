namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Admin;

public class ConcurrencyPolicyConfiguration : IEntityTypeConfiguration<ConcurrencyPolicy>
{
    public void Configure(EntityTypeBuilder<ConcurrencyPolicy> builder)
    {
        builder.ToTable("concurrency_policies");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.OperationType).HasMaxLength(50).IsRequired();
        builder.Property(e => e.LockingStrategy).HasConversion<string>().HasMaxLength(20);
    }
}
