namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards;

public sealed record FlashcardMediaContextDto(
	Guid VideoId,
	string VideoTitle,
	string? VideoUrl,
	TimeSpan? StartTime,
	TimeSpan? EndTime,
	string? OriginalSentence,
	string? TranslatedSentence,
	int? StartTokenIndex,
	int? EndTokenIndex);

public sealed record FlashcardItemDto(
	Guid SavedItemId,
	string Expression,
	string? Reading,
	string Meaning,
	IReadOnlyList<FlashcardMediaContextDto> VideoContexts);

public sealed record FlashcardSessionDetailDto(
	Guid SessionId,
	string Status,
	DateTimeOffset CreatedAt,
	DateTimeOffset ExpiresAt,
	DateTimeOffset? CompletedAt,
	int TotalItems,
	IReadOnlyList<FlashcardItemDto> Items);

public sealed record FlashcardItemRatingInput(
	Guid SavedItemId,
	string Rating);

public sealed record FlashcardItemResultDto(
	Guid SavedItemId,
	string Expression,
	string Rating,
	int Quality,
	int IntervalDays,
	int Repetitions,
	DateTimeOffset NextDueAt,
	string NewStatus);

public sealed record FlashcardSessionResultDto(
	Guid SessionId,
	string Status,
	DateTimeOffset CompletedAt,
	int TotalItems,
	int ForgotCount,
	int HardCount,
	int EasyCount,
	IReadOnlyList<FlashcardItemResultDto> Results);
