namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Admin;

public class AdminOverrideConfiguration : IEntityTypeConfiguration<AdminOverride>
{
    public void Configure(EntityTypeBuilder<AdminOverride> builder)
    {
        builder.ToTable("admin_overrides");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.ActionType).HasConversion<string>().HasMaxLength(30);
        builder.Property(e => e.RefType).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Reason).HasMaxLength(1000).IsRequired();
        builder.Property(e => e.Details).HasMaxLength(4000);
        builder.HasIndex(e => new { e.RefType, e.RefId });
        builder.HasIndex(e => e.FourCastoWlId);
    }
}
