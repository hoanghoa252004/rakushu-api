namespace Rakushu.Application.Usecases.Learning.Srs;

public sealed record SrsItemDto(
	Guid Id,
	Guid MeaningId,
	string Expression,
	string? Reading,
	string Meaning,
	string Status,
	DateTimeOffset? DueAt,
	DateTimeOffset? LastReviewedAt,
	DateTimeOffset SavedAt);
