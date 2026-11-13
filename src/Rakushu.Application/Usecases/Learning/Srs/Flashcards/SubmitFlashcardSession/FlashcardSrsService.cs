using Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.SubmitQuizAttempt;
using Rakushu.Domain.Entities.Srs.FlashcardSession;
using Rakushu.Domain.Entities.Srs.SavedItem;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsScheduleDetail;

namespace Rakushu.Application.Usecases.Learning.Srs.Flashcards.SubmitFlashcardSession;

internal sealed class FlashcardSrsService : IFlashcardSrsService
{
	private readonly ISavedItemRepository _savedItemRepository;

	public FlashcardSrsService(ISavedItemRepository savedItemRepository)
	{
		_savedItemRepository = savedItemRepository;
	}

	public async Task ApplySrsUpdatesAsync(
		FlashcardSession session,
		DateTimeOffset now,
		CancellationToken cancellationToken = default)
	{
		var reviewedItems = session.Items.ToList();
		if (reviewedItems.Count == 0)
			return;

		var savedItemIds = reviewedItems
			.Select(i => i.SavedItemId)
			.Distinct()
			.ToList();

		var savedItems = await _savedItemRepository.GetByIdsWithScheduleAsync(
			savedItemIds,
			session.UserId,
			cancellationToken);

		var savedItemDict = savedItems.ToDictionary(si => si.Id);

		foreach (var item in reviewedItems)
		{
			if (!savedItemDict.TryGetValue(item.SavedItemId, out var savedItem))
				continue;

			int quality = (int)item.Rating;

			var schedule = savedItem.Schedule;
			if (schedule is null)
			{
				var createSchedResult = SrsSchedule.Create(savedItem.Id, now);
				if (createSchedResult.IsSuccess)
				{
					schedule = createSchedResult.Value;
					savedItem.AttachSchedule(schedule);
				}
			}

			if (schedule is null)
				continue;

			var lastDetail = schedule.Details
				.OrderByDescending(d => d.ReviewNumber)
				.FirstOrDefault();

			int lastRepetitions = lastDetail?.Repetitions ?? 0;
			int lastInterval = lastDetail?.IntervalDays ?? 0;
			double lastFactor = lastDetail?.EasinessFactor ?? 2.5;
			int lastLapses = lastDetail?.Lapses ?? 0;
			int nextReviewNumber = (lastDetail?.ReviewNumber ?? 0) + 1;

			var sm2 = Sm2Calculator.Calculate(lastRepetitions, lastInterval, lastFactor, lastLapses, quality, now);
			schedule.UpdateSchedule(sm2.DueAt, now);

			var detailResult = SrsScheduleDetail.Create(
				schedule.Id,
				nextReviewNumber,
				quality,
				sm2.IntervalDays,
				sm2.EasinessFactor,
				sm2.Repetitions,
				sm2.Lapses,
				now,
				quizAttemptItemId: null,
				flashcardSessionItemId: item.Id);

			if (detailResult.IsSuccess)
			{
				schedule.AddDetail(detailResult.Value);
			}
		}
	}
}
