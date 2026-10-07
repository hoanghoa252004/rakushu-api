using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rakushu.Domain.Entities.LearningUnit;
using Rakushu.Domain.Entities.LinguisticKnowledge;
using Rakushu.Domain.Entities.Video.Transcript.TranscriptSegment;

namespace Rakushu.Persistence.Configurations;

internal sealed class LearningUnitConfiguration : IEntityTypeConfiguration<LearningUnit>
{
	public void Configure(EntityTypeBuilder<LearningUnit> builder)
	{
		// Id
		builder.HasKey(l => l.Id);
		builder.Property(l => l.Id)
			.HasConversion(
				id => id.Value,
				value => LearningUnitId.From(value));

		// LinguisticKnowledgeId
		builder.Property(l => l.LinguisticKnowledgeId)
			.HasConversion(
				id => id.Value,
				value => LinguisticKnowledgeId.From(value))
			.IsRequired();
		builder.HasOne(l => l.LinguisticKnowledge)
			.WithMany(xx => xx.LearningUnits)
			.HasForeignKey(l => l.LinguisticKnowledgeId)
			.OnDelete(DeleteBehavior.Restrict);

		// TranscriptSegmentId
		builder.Property(l => l.TranscriptSegmentId)
			.HasConversion(
				id => id.Value,
				value => TranscriptSegmentId.From(value))
			.IsRequired();
		builder.HasOne(l => l.TranscriptSegment)
			.WithMany(xx => xx.LearningUnits)
			.HasForeignKey(l => l.TranscriptSegmentId)
			.OnDelete(DeleteBehavior.Restrict);

		// StartTokenIndex
		builder.Property(l => l.StartTokenIndex)
			.IsRequired();

		// EndTokenIndex
		builder.Property(l => l.EndTokenIndex)
			.IsRequired();

		// Confidence
		builder.Property(l => l.Confidence)
			.IsRequired();

		// DetectionMethod
		builder.Property(l => l.DetectionMethod)
			.HasMaxLength(30)
			.HasConversion<string>()
			.IsRequired();

		// DetectedAt
		builder.Property(l => l.DetectedAt)
			.IsRequired();

		// UpdatedAt
		builder.Property(l => l.UpdatedAt)
			.IsRequired();
	}
}
