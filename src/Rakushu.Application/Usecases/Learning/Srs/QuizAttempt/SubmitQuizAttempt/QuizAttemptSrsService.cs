using Rakushu.Domain.Entities.Srs.QuizAttempt;
using Rakushu.Domain.Entities.Srs.SavedItem;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsScheduleDetail;
using QuizAttemptEntity = Rakushu.Domain.Entities.Srs.QuizAttempt.QuizAttempt;

namespace Rakushu.Application.Usecases.Learning.Srs.QuizAttempt.SubmitQuizAttempt;

public interface IQuizAttemptSrsService
{
	Task ApplySrsUpdatesAsync(
		QuizAttemptEntity attempt,
		DateTimeOffset now,
		CancellationToken cancellationToken = default);
}

internal sealed class QuizAttemptSrsService : IQuizAttemptSrsService
{
	private readonly ISavedItemRepository _savedItemRepository;

	public QuizAttemptSrsService(ISavedItemRepository savedItemRepository)
	{
		_savedItemRepository = savedItemRepository;
	}

	public async Task ApplySrsUpdatesAsync(
		QuizAttemptEntity attempt,
		DateTimeOffset now,
		CancellationToken cancellationToken = default)
	{
		if (attempt.Mode != QuizAttemptMode.SrsReview)
			return;

		var completedItems = attempt.Items.Where(i => i.AnsweredAt.HasValue).ToList();
		if (completedItems.Count == 0)
			return;

		var questionMap = attempt.Quiz.Questions.ToDictionary(q => q.Id);
		var savedItemIds = completedItems
			.Select(i => questionMap.TryGetValue(i.QuestionId, out var q) ? q.SavedItemId : null)
			.Where(id => id is not null)
			.Select(id => id!)
			.Distinct()
			.ToList();

		if (savedItemIds.Count == 0)
			return;

		var savedItems = await _savedItemRepository.GetByIdsWithScheduleAsync(
			savedItemIds,
			attempt.UserId,
			cancellationToken);

		var savedItemDict = savedItems.ToDictionary(si => si.Id);

		foreach (var item in completedItems)
		{
			if (!questionMap.TryGetValue(item.QuestionId, out var question))
				continue;

			if (!savedItemDict.TryGetValue(question.SavedItemId, out var savedItem))
				continue;

			bool isCorrect = item.IsCorrect ?? false;
			int timeSpentMs = item.TimeSpentMs;

			int quality = !isCorrect ? 1
				: timeSpentMs > 15_000 ? 3
				: timeSpentMs >= 5_000 ? 4
				: 5;

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
				quizAttemptItemId: item.Id);

			if (detailResult.IsSuccess)
			{
				schedule.AddDetail(detailResult.Value);
			}

			_savedItemRepository.Update(savedItem);
		}
	}
}
