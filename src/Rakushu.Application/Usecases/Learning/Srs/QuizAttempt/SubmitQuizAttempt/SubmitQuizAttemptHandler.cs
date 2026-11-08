using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.QuizAttempt;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.SubmitQuizAttempt;

internal sealed class SubmitQuizAttemptHandler : IRequestHandler<SubmitQuizAttemptCommand, Result>
{
	private readonly IQuizAttemptRepository _quizAttemptRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IQuizAttemptSrsService _quizAttemptSrsService;
	private readonly IServiceScopeFactory _scopeFactory;
	private readonly IUnitOfWork _unitOfWork;

	public SubmitQuizAttemptHandler(
		IQuizAttemptRepository quizAttemptRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IQuizAttemptSrsService quizAttemptSrsService,
		IServiceScopeFactory scopeFactory,
		IUnitOfWork unitOfWork)
	{
		_quizAttemptRepository = quizAttemptRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_quizAttemptSrsService = quizAttemptSrsService;
		_scopeFactory = scopeFactory;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		SubmitQuizAttemptCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure(UserErrors.NotFound);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var attemptId = QuizAttemptId.From(request.AttemptId);
			var attempt = await _quizAttemptRepository.GetByIdWithItemsAndQuizAsync(attemptId, cancellationToken);
			if (attempt is null || attempt.UserId != userId)
				return Result.Failure(QuizAttemptErrors.NotFound);

			if (attempt.Status != QuizAttemptStatus.InProgress)
				return Result.Failure(QuizAttemptErrors.NotInProgress);

			var now = _systemClock.UtcNow;

			if (attempt.IsExpired(now))
			{
				if (!attempt.HasAnyAnsweredItems)
				{
					_quizAttemptRepository.Delete(attempt);
				}
				else
				{
					var unrated = attempt.ExpireAndPrune(now);
					if (unrated.Count > 0)
					{
						_quizAttemptRepository.RemoveItems(unrated);
					}

					await _quizAttemptSrsService.ApplySrsUpdatesAsync(attempt, now, cancellationToken);
					_quizAttemptRepository.Update(attempt);
				}

				return Result.Failure(QuizAttemptErrors.AttemptExpired);
			}

			if (!attempt.HasAnyAnsweredItems)
			{
				return Result.Failure(QuizAttemptErrors.NoItemsAnswered);
			}

			var unansweredItems = attempt.PruneUnansweredItems();
			if (unansweredItems.Count > 0)
			{
				_quizAttemptRepository.RemoveItems(unansweredItems);
			}

			await _quizAttemptSrsService.ApplySrsUpdatesAsync(attempt, now, cancellationToken);

			var completeResult = attempt.Complete(now);
			if (completeResult.IsFailure)
				return completeResult;

			_quizAttemptRepository.Update(attempt);

			_ = Task.Run(async () =>
			{
				try
				{
					using var scope = _scopeFactory.CreateScope();
					var feedbackService = scope.ServiceProvider.GetRequiredService<IQuizAttemptFeedbackService>();
					await feedbackService.GenerateFeedbackAsync(attemptId, CancellationToken.None);
				}
				catch
				{
					// Ignored or logged within feedbackService
				}
			});

			return Result.Success();
		}, cancellationToken);
	}
}
