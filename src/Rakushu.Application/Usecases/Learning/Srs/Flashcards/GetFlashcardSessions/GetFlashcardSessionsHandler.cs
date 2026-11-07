using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Common.Pagination;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.FlashcardSession;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.GetFlashcardSessions;

internal sealed class GetFlashcardSessionsHandler : IRequestHandler<GetFlashcardSessionsQuery, Result<PaginatedList<FlashcardSessionSummaryDto>>>
{
	private readonly IFlashcardSessionRepository _flashcardSessionRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;

	public GetFlashcardSessionsHandler(
		IFlashcardSessionRepository flashcardSessionRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock)
	{
		_flashcardSessionRepository = flashcardSessionRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
	}

	public async Task<Result<PaginatedList<FlashcardSessionSummaryDto>>> Handle(
		GetFlashcardSessionsQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<PaginatedList<FlashcardSessionSummaryDto>>(UserErrors.NotFound);

		FlashcardSessionStatus? statusFilter = null;
		if (!string.IsNullOrWhiteSpace(request.Status) &&
			Enum.TryParse<FlashcardSessionStatus>(request.Status, true, out var parsedStatus))
		{
			statusFilter = parsedStatus;
		}

		var (sessions, totalCount) = await _flashcardSessionRepository.GetSessionsByUserAsync(
			userId,
			statusFilter,
			request.PageNumber,
			request.PageSize,
			cancellationToken);

		var now = _systemClock.UtcNow;

		var dtoList = sessions.Select(s =>
		{
			var currentStatus = s.Status;
			if (s.IsExpired(now))
			{
				currentStatus = FlashcardSessionStatus.Expired;
			}

			return new FlashcardSessionSummaryDto(
				s.Id.Value,
				currentStatus.ToString(),
				s.Items.Count,
				s.Items.Count(i => i.Rating.HasValue),
				s.CreatedAt,
				s.ExpiresAt,
				s.CompletedAt);
		}).ToList();

		return Result.Success(new PaginatedList<FlashcardSessionSummaryDto>(
			dtoList,
			totalCount,
			request.PageNumber,
			request.PageSize));
	}
}
