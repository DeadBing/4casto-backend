namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Admin;

public class UserNotificationPreferenceConfiguration : IEntityTypeConfiguration<UserNotificationPreference>
{
    public void Configure(EntityTypeBuilder<UserNotificationPreference> builder)
    {
        builder.ToTable("user_notification_preferences");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.EventType).HasConversion<string>().HasMaxLength(40);
        builder.Property(e => e.Channel).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(e => new { e.UserId, e.EventType, e.Channel }).IsUnique();
    }
}
