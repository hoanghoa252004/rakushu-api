using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.LearningUnit;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Persistence.Configurations.Srs;

internal sealed class SrsNoteConfiguration : IEntityTypeConfiguration<SrsNote>
{
    public void Configure(EntityTypeBuilder<SrsNote> builder)
    {
        // Schema and table
        builder.ToTable("srs_notes", "srs");

        // Id
        builder.HasKey(note => note.Id);

        // UserId
        builder.Property(note => note.UserId)
            .HasConversion(
                id => id.Value,
                value => UserId.From(value))
            .IsRequired();

        // LearningUnitId
        builder.Property(note => note.LearningUnitId)
            .HasConversion(
                id => id.Value,
                value => LearningUnitId.From(value))
            .IsRequired();

        // Timestamps
        builder.Property(note => note.CreatedAt)
            .IsRequired();

        builder.Property(note => note.UpdatedAt)
            .IsRequired();

        // Relationships
        builder.HasOne(note => note.User)
            .WithMany()
            .HasForeignKey(note => note.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(note => note.LearningUnit)
            .WithMany()
            .HasForeignKey(note => note.LearningUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(note => new { note.UserId, note.CreatedAt });

        builder.HasIndex(note => new { note.UserId, note.LearningUnitId });
    }
}