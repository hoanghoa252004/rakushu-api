using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Material.Video;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;

namespace Rakushu.Persistence.Configurations.Shadowing;

internal sealed class ShadowingMaterialConfiguration : IEntityTypeConfiguration<ShadowingMaterial>
{
	public void Configure(EntityTypeBuilder<ShadowingMaterial> builder)
	{
		// Schema
		builder.Metadata.SetSchema("shadowing");

		// Id
		builder.HasKey(m => m.Id);
		builder.Property(m => m.Id)
			.HasConversion(
				id => id.Value,
				value => ShadowingMaterialId.From(value));

		// Name
		builder.Property(m => m.Name)
			.HasMaxLength(200)
			.IsRequired();

		// Start / End transcript segment
		builder.Property(m => m.StartTranscriptSegmentId)
			.HasConversion(
				id => id.Value,
				value => TranscriptSegmentId.From(value));
		builder.HasOne(m => m.StartTranscriptSegment)
			.WithMany(s => s.StartingShadowingMaterials)
			.HasForeignKey(m => m.StartTranscriptSegmentId)
			.OnDelete(DeleteBehavior.SetNull);
		builder.Property(m => m.EndTranscriptSegmentId)
			.HasConversion(
				id => id.Value,
				value => TranscriptSegmentId.From(value));
		builder.HasOne(m => m.EndTranscriptSegment)
			.WithMany(s => s.EndingShadowingMaterials)
			.HasForeignKey(m => m.EndTranscriptSegmentId)
			.OnDelete(DeleteBehavior.SetNull);
		//builder.HasIndex(m => new { m.StartTranscriptSegmentId, m.EndTranscriptSegmentId })
		//	.IsUnique();

		// Status (enum)
		builder.Property(s => s.Status)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// VideoId
		builder.Property(m => m.VideoId)
			.HasConversion(
				id => id.Value,
				value => VideoId.From(value))
			.IsRequired();
		builder.HasOne<Video>(x => x.Video)
			.WithMany(v => v.ShadowingMaterials)
			.HasForeignKey(m => m.VideoId)
			.OnDelete(DeleteBehavior.Restrict);

		// Version
		builder.Property(m => m.Version)
			.IsRequired();
		builder.HasIndex(m => new { m.VideoId, m.Version })
			.IsUnique();

		// CreatedById
		builder.Property(m => m.CreatedById)
			.HasConversion(
				id => id.Value,
				value => UserId.From(value))
			.IsRequired();
		builder.HasOne(m => m.CreatedBy)
			.WithMany()
			.HasForeignKey(m => m.CreatedById)
			.OnDelete(DeleteBehavior.Restrict);

		// CreatedAt
		builder.Property(m => m.CreatedAt)
			.IsRequired();
	}
}
