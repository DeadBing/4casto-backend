namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Markets;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.ToTable("subjects");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Symbol).HasMaxLength(50).IsRequired();
        builder.HasIndex(e => new { e.FourCastoWlId, e.Symbol }).IsUnique();
        builder.HasOne(e => e.SubjectGroup).WithMany(g => g.Subjects).HasForeignKey(e => e.SubjectGroupId);
    }
}
