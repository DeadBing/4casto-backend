namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Identity;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Email).HasMaxLength(320).IsRequired();
        builder.Property(e => e.PasswordHash).HasMaxLength(500);
        builder.Property(e => e.AuthProvider).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(e => new { e.FourCastoWlId, e.Email }).IsUnique();
        builder.HasOne(e => e.FourCastoWl).WithMany().HasForeignKey(e => e.FourCastoWlId);
        builder.HasOne(e => e.Profile).WithOne(p => p.User).HasForeignKey<UserProfile>(p => p.UserId);
    }
}
