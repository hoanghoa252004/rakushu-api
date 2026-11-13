using MediatR;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.SavedItem.GetSrsItems;

public sealed record GetSrsItemsQuery(
	bool? IsActive = null,
	bool? DueOnly = null,
	int Page = 1,
	int PageSize = 50) : IRequest<Result<IReadOnlyList<SavedItemSummaryDto>>>;
