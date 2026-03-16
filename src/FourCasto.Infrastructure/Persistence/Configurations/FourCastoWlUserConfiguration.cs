namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Identity;

public class FourCastoWlUserConfiguration : IEntityTypeConfiguration<FourCastoWlUser>
{
    public void Configure(EntityTypeBuilder<FourCastoWlUser> builder)
    {
        builder.ToTable("fourcasto_wl_users");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Role).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(e => new { e.FourCastoWlId, e.UserId }).IsUnique();
        builder.HasOne(e => e.FourCastoWl).WithMany().HasForeignKey(e => e.FourCastoWlId);
        builder.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);
    }
}
