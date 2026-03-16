namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.CountryRules;

public class UserCountryStatusAssignmentConfiguration : IEntityTypeConfiguration<UserCountryStatusAssignment>
{
    public void Configure(EntityTypeBuilder<UserCountryStatusAssignment> builder)
    {
        builder.ToTable("user_country_status_assignments");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.CountryCode).HasMaxLength(3).IsRequired();
        builder.HasIndex(e => new { e.FourCastoWlId, e.UserId }).IsUnique();
        builder.HasOne(e => e.CountryStatus).WithMany().HasForeignKey(e => e.CountryStatusId);
    }
}
