using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.FlashcardSession;
using Rakushu.Domain.Entities.Srs.QuizAttempt;

namespace Rakushu.Application.Usecases.Learning.Srs.History.GetSrsHistorySummary;

internal sealed class GetSrsHistorySummaryHandler : IRequestHandler<GetSrsHistorySummaryQuery, Result<SrsHistorySummaryDto>>
{
	private readonly IFlashcardSessionRepository _flashcardSessionRepository;
	private readonly IQuizAttemptRepository _quizAttemptRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;

	public GetSrsHistorySummaryHandler(
		IFlashcardSessionRepository flashcardSessionRepository,
		IQuizAttemptRepository quizAttemptRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock)
	{
		_flashcardSessionRepository = flashcardSessionRepository;
		_quizAttemptRepository = quizAttemptRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
	}

	public async Task<Result<SrsHistorySummaryDto>> Handle(
		GetSrsHistorySummaryQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
		{
			return Result.Failure<SrsHistorySummaryDto>(UserErrors.NotFound);
		}

		var now = _systemClock.UtcNow;

		// DbContext is scoped and not thread-safe; execute repository aggregations sequentially.
		var flashcardSummary = await _flashcardSessionRepository.GetRatingSummaryByUserAsync(
			userId,
			cancellationToken);

		var quizSummary = await _quizAttemptRepository.GetAnswerSummaryByUserAsync(
			userId,
			now,
			cancellationToken);

		var dto = new SrsHistorySummaryDto(
			new FlashcardHistorySummaryDto(
				flashcardSummary.EasyCount,
				flashcardSummary.HardCount,
				flashcardSummary.ForgotCount),
			new QuizHistorySummaryDto(
				quizSummary.CorrectCount,
				quizSummary.IncorrectCount,
				quizSummary.UnansweredCount));

		return Result.Success(dto);
	}
}
