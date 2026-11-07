namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards;

public sealed record FlashcardMediaContextDto(
	Guid LearningUnitId,
	string VideoTitle,
	string? VideoUrl,
	TimeSpan? StartTime,
	TimeSpan? EndTime,
	string? OriginalSentence,
	string? TranslatedSentence,
	int? StartTokenIndex,
	int? EndTokenIndex);

public sealed record FlashcardItemDto(
	Guid ItemId,
	Guid SavedItemId,
	string Expression,
	string? Reading,
	string Meaning,
	string? Rating,
	DateTimeOffset? ReviewedAt,
	IReadOnlyList<FlashcardMediaContextDto> VideoContexts);

public sealed record FlashcardSessionDetailDto(
	Guid SessionId,
	string Status,
	DateTimeOffset CreatedAt,
	DateTimeOffset ExpiresAt,
	DateTimeOffset? CompletedAt,
	int TotalItems,
	IReadOnlyList<FlashcardItemDto> Items);

public sealed record FlashcardSessionSummaryDto(
	Guid Id,
	string Status,
	int TotalItems,
	int CompletedItems,
	DateTimeOffset CreatedAt,
	DateTimeOffset ExpiresAt,
	DateTimeOffset? CompletedAt);

public sealed record SubmitFlashcardItemRatingInput(
	string Rating);

public sealed record FlashcardItemResultDto(
	Guid SavedItemId,
	string Expression,
	string? Reading,
	string Meaning,
	string Rating,
	int Quality,
	int IntervalDays,
	int Repetitions,
	DateTimeOffset NextDueAt);

public sealed record FlashcardSessionResultDto(
	Guid SessionId,
	string Status,
	DateTimeOffset? CompletedAt,
	int TotalItems,
	int ForgotCount,
	int HardCount,
	int EasyCount,
	IReadOnlyList<FlashcardItemResultDto> Results);
