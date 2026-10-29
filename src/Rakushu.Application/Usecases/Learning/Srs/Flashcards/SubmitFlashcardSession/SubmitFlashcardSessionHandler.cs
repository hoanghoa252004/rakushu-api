using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Learning.Srs.SubmitQuizAttempt;
using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.FlashcardSession;
using Rakushu.Domain.Entities.Srs.FlashcardSession.FlashcardSessionItem;
using Rakushu.Domain.Entities.Srs.SavedItem;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsItemDetail;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.SubmitFlashcardSession;

internal sealed class SubmitFlashcardSessionHandler : IRequestHandler<SubmitFlashcardSessionCommand, Result<FlashcardSessionResultDto>>
{
	private readonly IFlashcardSessionRepository _flashcardSessionRepository;
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;
	private readonly IUnitOfWork _unitOfWork;

	public SubmitFlashcardSessionHandler(
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

	public async Task<Result<FlashcardSessionResultDto>> Handle(
		SubmitFlashcardSessionCommand request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<FlashcardSessionResultDto>(UserErrors.NotFound);

		return await _unitOfWork.ExecuteAsync(async () =>
		{
			var now = _systemClock.UtcNow;

			var session = await _flashcardSessionRepository.GetByIdWithItemsAsync(
				FlashcardSessionId.From(request.SessionId),
				cancellationToken);

			if (session is null)
				return Result.Failure<FlashcardSessionResultDto>(FlashcardSessionErrors.NotFound);

			if (session.UserId != userId)
				return Result.Failure<FlashcardSessionResultDto>(FlashcardSessionErrors.NotFound);

			if (session.Status != FlashcardSessionStatus.InProgress)
				return Result.Failure<FlashcardSessionResultDto>(FlashcardSessionErrors.NotInProgress);

			if (session.IsExpired(now))
			{
				session.Cancel(now);
				_flashcardSessionRepository.Update(session);
				return Result.Failure<FlashcardSessionResultDto>(FlashcardSessionErrors.Expired);
			}

			// Validate all session items are accounted for
			var ratingsBySavedItemId = request.Ratings
				.ToDictionary(r => r.SavedItemId, r => r.Rating);

			if (ratingsBySavedItemId.Count != session.Items.Count)
				return Result.Failure<FlashcardSessionResultDto>(FlashcardSessionErrors.IncompleteRatings);

			// Load saved items with their schedules
			var savedItemIds = session.Items.Select(i => i.SavedItemId).ToList();
			var savedItems = await _savedItemRepository.GetByIdsWithScheduleAsync(savedItemIds, userId, cancellationToken);
			var savedItemsById = savedItems.ToDictionary(i => i.Id.Value);

			var results = new List<FlashcardItemResultDto>(session.Items.Count);
			int forgotCount = 0;
			int hardCount = 0;
			int easyCount = 0;

			foreach (var sessionItem in session.Items)
			{
				if (!ratingsBySavedItemId.TryGetValue(sessionItem.SavedItemId.Value, out var ratingStr))
					return Result.Failure<FlashcardSessionResultDto>(FlashcardSessionErrors.IncompleteRatings);

				if (!savedItemsById.TryGetValue(sessionItem.SavedItemId.Value, out var savedItem))
					return Result.Failure<FlashcardSessionResultDto>(SavedItemErrors.NotFound);

				var rating = ParseRating(ratingStr);
				var quality = (int)rating;

				sessionItem.RecordRating(rating, now);

				if (rating == FlashcardRating.Forgot)
					forgotCount++;
				else if (rating == FlashcardRating.Hard)
					hardCount++;
				else
					easyCount++;

				// SM-2 Schedule Calculation
				var schedule = savedItem.Schedule;
				if (schedule is null)
				{
					var createSchedResult = SrsSchedule.Create(savedItem.Id, now);
					if (createSchedResult.IsFailure)
						return Result.Failure<FlashcardSessionResultDto>(createSchedResult.Error);

					schedule = createSchedResult.Value;
					savedItem.AttachSchedule(schedule);
				}

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

				schedule.UpdateSchedule(sm2.NewStatus, sm2.DueAt, now);

				var detailResult = SrsItemDetail.Create(
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
					return Result.Failure<FlashcardSessionResultDto>(detailResult.Error);

				schedule.AddDetail(detailResult.Value);

				results.Add(new FlashcardItemResultDto(
					savedItem.Id.Value,
					savedItem.Expression,
					rating.ToString(),
					quality,
					sm2.IntervalDays,
					sm2.Repetitions,
					sm2.DueAt,
					sm2.NewStatus.ToString()));
			}

			var submitResult = session.Submit(now);
			if (submitResult.IsFailure)
				return Result.Failure<FlashcardSessionResultDto>(submitResult.Error);

			_flashcardSessionRepository.Update(session);

			return Result.Success(new FlashcardSessionResultDto(
				session.Id.Value,
				session.Status.ToString(),
				session.CompletedAt ?? now,
				session.Items.Count,
				forgotCount,
				hardCount,
				easyCount,
				results));
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
