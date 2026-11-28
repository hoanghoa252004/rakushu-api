using System.Text.Json.Serialization;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

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

public sealed record SavedItemSummaryDto(
	Guid Id,
	Guid MeaningId,
	string Expression,
	string? Reading,
	string Meaning,
	bool IsActive,
	DateTimeOffset? DueAt,
	DateTimeOffset SavedAt,
	bool IsRevoked = false,
	int? Lapses = null,
	double? EasinessFactor = null,
	int? IntervalDays = null,
	int? Repetitions = null,
	int RecentEncounterCount = 0,
	string? JlptLevel = null,
	string? KnowledgeType = null,
	string? Status = null,
	bool IsMastered = false);

public sealed record SavedItemDetailDto(
	Guid Id,
	Guid MeaningId,
	string Expression,
	string? Reading,
	string Meaning,
	bool IsActive,
	DateTimeOffset SavedAt,
	IReadOnlyList<SrsItemVideoContextDto> VideoContexts,
	bool IsRevoked = false);

public sealed record ScheduleDetailHistoryDto(
	int ReviewNumber,
	DateTimeOffset ReviewedAt,
	int Quality,
	int IntervalDays,
	double EasinessFactor,
	int Repetitions,
	int Lapses,
	[property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	Guid? QuizAttemptId = null,
	[property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	Guid? FlashcardSessionId = null);

public sealed record ImmersionBoostHistoryDto(
	Guid Id,
	string AdjustmentType,
	int BonusDays,
	DateTimeOffset PreviousDueAt,
	DateTimeOffset NewDueAt,
	int BoostNumber,
	int EncounterCountInWindow,
	DateTimeOffset BoostedAt);

public sealed record SavedItemScheduleDto(
	Guid SavedItemId,
	DateTimeOffset? DueAt,
	int CurrentIntervalDays,
	double CurrentEasinessFactor,
	int CurrentRepetitions,
	int CurrentLapses,
	IReadOnlyList<ScheduleDetailHistoryDto> ReviewHistory,
	DateTimeOffset? LastImmersionBoostAt = null,
	int ConsecutivePassiveBoosts = 0,
	int MaxConsecutivePassiveBoosts = SrsSchedule.MaxConsecutivePassiveBoosts,
	IReadOnlyList<ImmersionBoostHistoryDto>? BoostHistory = null);

// Backwards-compatible alias for existing code referencing SrsItemDto
public sealed record SrsItemDto(
	Guid Id,
	Guid MeaningId,
	string Expression,
	string? Reading,
	string Meaning,
	bool IsActive,
	DateTimeOffset? DueAt,
	DateTimeOffset SavedAt,
	IReadOnlyList<SrsItemVideoContextDto> VideoContexts,
	bool IsRevoked = false)
{
	public SrsItemDto(
		Guid id,
		Guid meaningId,
		string expression,
		string? reading,
		string meaning,
		bool isActive,
		DateTimeOffset? dueAt,
		DateTimeOffset savedAt)
		: this(id, meaningId, expression, reading, meaning, isActive, dueAt, savedAt, Array.Empty<SrsItemVideoContextDto>(), false)
	{
	}
}
