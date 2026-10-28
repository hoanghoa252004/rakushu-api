using MediatR;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.GetSrsItems;

public sealed record GetSrsItemsQuery(
	string? Status = null,
	int Page = 1,
	int PageSize = 50) : IRequest<Result<IReadOnlyList<SrsItemDto>>>;
