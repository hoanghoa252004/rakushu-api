using MediatR;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Curation.GetMyIssueHistory;

public sealed record GetMyIssueHistoryQuery(
	int Page = 1,
	int PageSize = 20) : IRequest<Result<IReadOnlyList<LearnerIssueHistoryDto>>>;
