using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.Material.Transcript.TranscriptSegment;
using Rakushu.Domain.Entities.Shadowing.ShadowingMaterial;
using Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;

namespace Rakushu.Persistence.Configurations.Shadowing;

internal sealed class ShadowingRoleLineTranscriptSegmentConfiguration : IEntityTypeConfiguration<ShadowingRoleLineTranscriptSegment>
{
	public void Configure(EntityTypeBuilder<ShadowingRoleLineTranscriptSegment> builder)
	{
		// Schema
		builder.Metadata.SetSchema("shadowing");

		// Key
		builder.HasKey(x => new { x.ShadowingRoleLineId, x.TranscriptSegmentId });

		// ShadowingRoleLineId
		builder.Property(x => x.ShadowingRoleLineId)
			.HasConversion(
				id => id.Value,
				value => ShadowingRoleLineId.From(value))
			.IsRequired();

		// TranscriptSegmentId
		builder.Property(x => x.TranscriptSegmentId)
			.HasConversion(
				id => id.Value,
				value => TranscriptSegmentId.From(value))
			.IsRequired();
		builder.HasOne(x => x.TranscriptSegment)
			.WithMany()
			.HasForeignKey(x => x.TranscriptSegmentId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
