namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Identity;

public class FourCastoWlConfiguration : IEntityTypeConfiguration<FourCastoWl>
{
    public void Configure(EntityTypeBuilder<FourCastoWl> builder)
    {
        builder.ToTable("fourcasto_wls");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Slug).HasMaxLength(100).IsRequired();
        builder.HasIndex(e => e.Slug).IsUnique();
    }
}
