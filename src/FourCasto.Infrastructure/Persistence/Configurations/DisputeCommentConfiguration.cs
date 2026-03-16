namespace FourCasto.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FourCasto.Domain.Settlement;

public class DisputeCommentConfiguration : IEntityTypeConfiguration<DisputeComment>
{
    public void Configure(EntityTypeBuilder<DisputeComment> builder)
    {
        builder.ToTable("dispute_comments");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.AuthorRole).HasMaxLength(30).IsRequired();
        builder.Property(e => e.Comment).HasMaxLength(4000).IsRequired();
        builder.HasOne(e => e.DisputeCase).WithMany(d => d.Comments).HasForeignKey(e => e.DisputeCaseId);
    }
}
