namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Fraud;

public class UserRestrictionConfiguration : IEntityTypeConfiguration<UserRestriction>
{
    public void Configure(EntityTypeBuilder<UserRestriction> builder)
    {
        builder.ToTable("user_restrictions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.RestrictionType).HasConversion<string>().HasMaxLength(30);
        builder.Property(e => e.Reason).HasMaxLength(1000).IsRequired();
        builder.HasIndex(e => new { e.FourCastoWlId, e.UserId, e.IsActive });
    }
}
