using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Curation.Learner.GetMyIssueHistory;

public sealed record GetMyIssueHistoryQuery(
	int Page = 1,
	int PageSize = 20,
	string? Status = null,
	string? TargetType = null) : IRequest<Result<PaginatedList<LearnerIssueHistoryDto>>>;
