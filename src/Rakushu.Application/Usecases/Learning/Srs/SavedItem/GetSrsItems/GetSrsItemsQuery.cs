using MediatR;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.SavedItem.GetSrsItems;

public sealed record GetSrsItemsQuery(
	int Page = 1,
	int PageSize = 50,
	string? Filter = null,
	string? SortBy = null,
	string? SortOrder = null,
	string? Level = null,
	string? KnowledgeType = null,
	string? Status = null,
	string? Search = null,
	string? DueDate = null,
	string? TimeZone = null,
	bool? IsActive = null) : IRequest<Result<IReadOnlyList<SavedItemSummaryDto>>>;
