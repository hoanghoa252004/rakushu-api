using MediatR;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Clock;
using Rakushu.Application.Usecases.Learning.Srs;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Application.Usecases.Learning.Srs.SavedItem.GetSavedItemSchedule;

internal sealed class GetSavedItemScheduleHandler : IRequestHandler<GetSavedItemScheduleQuery, Result<SavedItemScheduleDto>>
{
	private readonly ISavedItemRepository _savedItemRepository;
	private readonly ICurrentUserContext _currentUserContext;
	private readonly ISystemClock _systemClock;

	public GetSavedItemScheduleHandler(
		ISavedItemRepository savedItemRepository,
		ICurrentUserContext currentUserContext,
		ISystemClock systemClock)
	{
		_savedItemRepository = savedItemRepository;
		_currentUserContext = currentUserContext;
		_systemClock = systemClock;
	}

	public async Task<Result<SavedItemScheduleDto>> Handle(
		GetSavedItemScheduleQuery request,
		CancellationToken cancellationToken)
	{
		var userId = _currentUserContext.UserId;
		if (userId is null)
			return Result.Failure<SavedItemScheduleDto>(UserErrors.NotFound);

		var itemId = SavedItemId.From(request.Id);
		var item = await _savedItemRepository.GetByIdWithScheduleAsync(itemId, cancellationToken);
		if (item is null || item.UserId != userId)
			return Result.Failure<SavedItemScheduleDto>(SavedItemErrors.NotFound);

		var schedule = item.Schedule;

		if (schedule is null)
		{
			return Result.Success(new SavedItemScheduleDto(
				item.Id.Value,
				null,
				0,
				2.5,
				0,
				0,
				Array.Empty<ScheduleDetailHistoryDto>()));
		}

		var sortedDetails = schedule.Details
			.OrderBy(d => d.ReviewNumber)
			.ToList();

		var latestDetail = sortedDetails.LastOrDefault();

		int currentIntervalDays = latestDetail?.IntervalDays ?? 0;
		double currentEasinessFactor = latestDetail?.EasinessFactor ?? 2.5;
		int currentRepetitions = latestDetail?.Repetitions ?? 0;
		int currentLapses = latestDetail?.Lapses ?? 0;

		var historyDtos = sortedDetails.Select(d =>
		{
			Guid? quizAttemptId = d.QuizAttemptItem?.QuizAttemptId?.Value;
			Guid? flashcardSessionId = d.FlashcardSessionItem?.FlashcardSessionId?.Value;

			return new ScheduleDetailHistoryDto(
				d.ReviewNumber,
				d.ReviewedAt,
				d.Quality,
				d.IntervalDays,
				d.EasinessFactor,
				d.Repetitions,
				d.Lapses,
				quizAttemptId,
				flashcardSessionId);
		}).ToList();

		return Result.Success(new SavedItemScheduleDto(
			item.Id.Value,
			schedule.DueAt,
			currentIntervalDays,
			currentEasinessFactor,
			currentRepetitions,
			currentLapses,
			historyDtos));
	}
}
