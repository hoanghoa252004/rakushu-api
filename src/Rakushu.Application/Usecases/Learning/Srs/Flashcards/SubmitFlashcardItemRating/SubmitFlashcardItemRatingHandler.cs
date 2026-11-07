using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.SubmitQuizAttempt;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.FlashcardSession;
using Rakushu.Domain.Entities.Srs.FlashcardSession.FlashcardSessionItem;
using Rakushu.Domain.Entities.Srs.SavedItem;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsScheduleDetail;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.SubmitFlashcardItemRating;

internal sealed class SubmitFlashcardItemRatingHandler : IRequestHandler<SubmitFlashcardItemRatingCommand, Result>
{
	private readonly IFlashcardSessionRepository _flashcardSessionRepository;
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public SubmitFlashcardItemRatingHandler(
		IFlashcardSessionRepository flashcardSessionRepository,
		ISavedItemRepository savedItemRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock,
		IUnitOfWork unitOfWork)
	{
		_flashcardSessionRepository = flashcardSessionRepository;
		_savedItemRepository = savedItemRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result> Handle(
		SubmitFlashcardItemRatingCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure(UserErrors.NotFound);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var now = _systemClock.UtcNow;

			var session = await _flashcardSessionRepository.GetByIdWithItemsAsync(
				FlashcardSessionId.From(request.SessionId),
				cancellationToken);

			if (session is null)
				return Result.Failure(FlashcardSessionErrors.NotFound);

			if (session.UserId != userId)
				return Result.Failure(FlashcardSessionErrors.NotFound);

			if (session.Status != FlashcardSessionStatus.InProgress)
				return Result.Failure(FlashcardSessionErrors.NotInProgress);

			if (session.IsExpired(now))
			{
				if (!session.HasAnyReviewedItems)
				{
					_flashcardSessionRepository.Delete(session);
				}
				else
				{
					var unrated = session.ExpireAndPrune(now);
					if (unrated.Count > 0)
					{
						_flashcardSessionRepository.RemoveItems(unrated);
					}
					_flashcardSessionRepository.Update(session);
				}
				return Result.Failure(FlashcardSessionErrors.Expired);
			}

			var sessionItem = session.Items.FirstOrDefault(i => i.Id.Value == request.ItemId);
			if (sessionItem is null)
				return Result.Failure(FlashcardSessionErrors.ItemNotFound);

			var rating = ParseRating(request.Rating);
			var quality = (int)rating;

			sessionItem.RecordRating(rating, now);
			session.RefreshIdleTimeout(now);

			var savedItem = await _savedItemRepository.GetByIdWithScheduleAsync(
				sessionItem.SavedItemId,
				cancellationToken);

			if (savedItem is null)
				return Result.Failure(SavedItemErrors.NotFound);

			var schedule = savedItem.Schedule;
			if (schedule is null)
			{
				var createSchedResult = SrsSchedule.Create(savedItem.Id, now);
				if (createSchedResult.IsFailure)
					return Result.Failure(createSchedResult.Error);

				schedule = createSchedResult.Value;
				savedItem.AttachSchedule(schedule);
			}

			var existingDetail = schedule.Details.FirstOrDefault(d => d.FlashcardSessionItemId == sessionItem.Id);
			if (existingDetail is null)
			{
				var lastDetail = schedule.Details
					.OrderByDescending(d => d.ReviewNumber)
					.FirstOrDefault();

				int lastRepetitions = lastDetail?.Repetitions ?? 0;
				int lastInterval = lastDetail?.IntervalDays ?? 0;
				double lastFactor = lastDetail?.EasinessFactor ?? 2.5;
				int lastLapses = lastDetail?.Lapses ?? 0;
				int reviewNumber = (lastDetail?.ReviewNumber ?? 0) + 1;

				var sm2 = Sm2Calculator.Calculate(
					lastRepetitions,
					lastInterval,
					lastFactor,
					lastLapses,
					quality,
					now);

				schedule.UpdateSchedule(sm2.DueAt, now);

				var detailResult = SrsScheduleDetail.Create(
					schedule.Id,
					reviewNumber,
					quality,
					sm2.IntervalDays,
					sm2.EasinessFactor,
					sm2.Repetitions,
					sm2.Lapses,
					now,
					quizAttemptItemId: null,
					flashcardSessionItemId: sessionItem.Id);

				if (detailResult.IsFailure)
					return Result.Failure(detailResult.Error);

				schedule.AddDetail(detailResult.Value);
			}

			_flashcardSessionRepository.Update(session);

			return Result.Success();
		}, cancellationToken);
	}

	private static FlashcardRating ParseRating(string ratingStr)
	{
		if (ratingStr.Equals("Forgot", StringComparison.OrdinalIgnoreCase))
			return FlashcardRating.Forgot;

		if (ratingStr.Equals("Hard", StringComparison.OrdinalIgnoreCase))
			return FlashcardRating.Hard;

		return FlashcardRating.Easy;
	}
}
