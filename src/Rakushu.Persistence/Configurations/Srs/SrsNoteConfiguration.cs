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
        builder.ToTable("srs_notes", "srs");
        builder.HasKey(note => note.Id);
        builder.Property(note => note.Id)
            .HasConversion(id => id.Value, value => SrsNoteId.From(value));

        builder.Property(note => note.UserId)
            .HasConversion(id => id.Value, value => UserId.From(value))
            .IsRequired();

        builder.Property(note => note.LearningUnitId)
            .HasConversion(id => id.Value, value => LearningUnitId.From(value))
            .IsRequired();

        builder.Property(note => note.CreatedAt).IsRequired();
        builder.Property(note => note.UpdatedAt).IsRequired();

        builder.HasOne(note => note.User)
            .WithMany()
            .HasForeignKey(note => note.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(note => note.LearningUnit)
            .WithMany()
            .HasForeignKey(note => note.LearningUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(note => new { note.UserId, note.CreatedAt });
        builder.HasIndex(note => new { note.UserId, note.LearningUnitId });
    }
}