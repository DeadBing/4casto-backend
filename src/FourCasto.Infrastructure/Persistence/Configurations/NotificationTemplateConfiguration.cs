namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Admin;

public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.ToTable("notification_templates");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.EventType).HasConversion<string>().HasMaxLength(40);
        builder.Property(e => e.Channel).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.TemplateBody).HasMaxLength(4000).IsRequired();
        builder.HasIndex(e => new { e.FourCastoWlId, e.EventType, e.Channel }).IsUnique();
    }
}
