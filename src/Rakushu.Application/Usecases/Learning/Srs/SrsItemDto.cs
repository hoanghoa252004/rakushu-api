namespace Rakushu.Application.Usecases.Learning.Srs;

public sealed record SrsItemVideoContextDto(
	Guid VideoId,
	string VideoTitle,
	string? VideoUrl,
	TimeSpan StartTime,
	TimeSpan EndTime,
	string OriginalSentence,
	string? TranslatedSentence,
	int? StartTokenIndex,
	int? EndTokenIndex,
	Guid? TranscriptSegmentId = null,
	Guid? LearningUnitId = null);

public sealed record SrsItemDto(
	Guid Id,
	Guid MeaningId,
	string Expression,
	string? Reading,
	string Meaning,
	string Status,
	DateTimeOffset? DueAt,
	DateTimeOffset? LastReviewedAt,
	DateTimeOffset SavedAt,
	IReadOnlyList<SrsItemVideoContextDto> VideoContexts)
{
	public SrsItemDto(
		Guid id,
		Guid meaningId,
		string expression,
		string? reading,
		string meaning,
		string status,
		DateTimeOffset? dueAt,
		DateTimeOffset? lastReviewedAt,
		DateTimeOffset savedAt)
		: this(id, meaningId, expression, reading, meaning, status, dueAt, lastReviewedAt, savedAt, Array.Empty<SrsItemVideoContextDto>())
	{
	}
}
