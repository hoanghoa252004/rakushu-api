using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.GetQuizAttempts;

public sealed record GetQuizAttemptsQuery(
	string? Status = null,
	int PageNumber = 1,
	int PageSize = 10) : IRequest<Result<PaginatedList<QuizAttemptSummaryDto>>>;
