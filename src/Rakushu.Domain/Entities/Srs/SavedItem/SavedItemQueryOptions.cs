namespace Rakushu.Domain.Entities.Srs.SavedItem;

public sealed record SavedItemQueryOptions(
	int Page = 1,
	int PageSize = 50,
	string? Filter = null,
	string? SortBy = null,
	string? SortOrder = null,
	string? Level = null,
	string? KnowledgeType = null,
	string? Status = null,
	string? Search = null,
	bool? IsActive = null,
	DateTimeOffset? DueFromUtc = null,
	DateTimeOffset? DueBeforeUtc = null,
	DateTimeOffset? EndOfTodayUtc = null,
	DateTimeOffset? NowUtc = null);
