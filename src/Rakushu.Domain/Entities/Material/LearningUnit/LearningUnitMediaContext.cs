namespace Rakushu.Domain.Entities.Material.LearningUnit;

public sealed record LearningUnitMediaContext(
	Guid MeaningId,
	Guid VideoId,
	string VideoTitle,
	string? StorageKey,
	string? SourceUrl,
	TimeSpan StartTime,
	TimeSpan EndTime,
	string OriginalSentence,
	string? TranslatedSentence,
	int StartTokenIndex,
	int EndTokenIndex,
	Guid? TranscriptSegmentId = null);
