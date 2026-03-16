namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Markets;

public class SubjectGroupConfiguration : IEntityTypeConfiguration<SubjectGroup>
{
    public void Configure(EntityTypeBuilder<SubjectGroup> builder)
    {
        builder.ToTable("subject_groups");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.HasIndex(e => new { e.FourCastoWlId, e.Name }).IsUnique();
    }
}
