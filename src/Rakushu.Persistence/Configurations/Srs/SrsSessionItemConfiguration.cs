using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class SrsSessionItemConfiguration : IEntityTypeConfiguration<SrsSessionItem>
{
    public void Configure(EntityTypeBuilder<SrsSessionItem> builder)
    {
        builder.ToTable("srs_session_items", "srs", table =>
        {
            table.HasCheckConstraint("ck_srs_session_items_position", "position > 0");
            table.HasCheckConstraint("ck_srs_session_items_elapsed_ms", "elapsed_ms IS NULL OR elapsed_ms >= 0");
        });
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Id).HasConversion(id => id.Value, value => SrsSessionItemId.From(value));
        builder.Property(item => item.SrsSessionId).HasConversion(id => id.Value, value => SrsSessionId.From(value));
        builder.Property(item => item.SrsCardId).HasConversion(id => id.Value, value => SrsCardId.From(value));
        builder.Property(item => item.Rating).HasMaxLength(20);
        builder.HasOne(item => item.SrsSession).WithMany(session => session.Items).HasForeignKey(item => item.SrsSessionId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(item => item.SrsCard).WithMany().HasForeignKey(item => item.SrsCardId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(item => new { item.SrsSessionId, item.Position }).IsUnique();
        builder.HasIndex(item => new { item.SrsSessionId, item.SrsCardId }).IsUnique();
    }
}
