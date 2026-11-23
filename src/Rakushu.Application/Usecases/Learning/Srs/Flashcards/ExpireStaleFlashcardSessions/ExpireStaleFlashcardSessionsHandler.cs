using MediatR;
using Microsoft.Extensions.Logging;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Learning.Srs.Flashcards.SubmitFlashcardSession;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Srs.FlashcardSession;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.ExpireStaleFlashcardSessions;

internal sealed class ExpireStaleFlashcardSessionsHandler : IRequestHandler<ExpireStaleFlashcardSessionsCommand, Result<int>>
{
	private readonly IFlashcardSessionRepository _flashcardSessionRepository;
	private readonly IFlashcardSrsService _flashcardSrsService;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ILogger<ExpireStaleFlashcardSessionsHandler> _logger;

	public ExpireStaleFlashcardSessionsHandler(
		IFlashcardSessionRepository flashcardSessionRepository,
		IFlashcardSrsService flashcardSrsService,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork,
		ILogger<ExpireStaleFlashcardSessionsHandler> logger)
	{
		_flashcardSessionRepository = flashcardSessionRepository;
		_flashcardSrsService = flashcardSrsService;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
		_logger = logger;
	}

	public async Task<Result<int>> Handle(
		ExpireStaleFlashcardSessionsCommand request,
		CancellationToken cancellationToken)
	{
		var now = _systemClock.UtcNow;
		var staleSessions = await _flashcardSessionRepository.GetStaleSessionsAsync(
			now,
			request.BatchSize,
			cancellationToken);

		if (staleSessions.Count == 0)
			return Result.Success(0);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			int expiredCount = 0;
			foreach (var session in staleSessions)
			{
				if (cancellationToken.IsCancellationRequested)
					break;

				try
				{
					session.Expire(now);
					if (session.HasAnyReviewedItems)
					{
						await _flashcardSrsService.ApplySrsUpdatesAsync(session, now, cancellationToken);
					}
					_flashcardSessionRepository.Update(session);
					expiredCount++;
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to expire stale flashcard session {SessionId}", session.Id.Value);
				}
			}

			return Result.Success(expiredCount);
		}, cancellationToken);
	}
}
