using MediatR;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.GetFlashcardSessions;

public sealed record GetFlashcardSessionsQuery(
	string? Status = null,
	int PageNumber = 1,
	int PageSize = 10) : IRequest<Result<PaginatedList<FlashcardSessionSummaryDto>>>;
