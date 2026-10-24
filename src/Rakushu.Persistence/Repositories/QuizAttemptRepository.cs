using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Persistence.Repositories;

public sealed class QuizAttemptRepository : BaseRepository<QuizAttempt, QuizAttemptId>, IQuizAttemptRepository
{
	public QuizAttemptRepository(RakushuDbContext context) : base(context) { }

	public Task<QuizAttempt?> GetForUserAsync(QuizAttemptId attemptId, UserId userId, CancellationToken cancellationToken = default)
	{
		return _context.QuizAttempts
			.Include(attempt => attempt.Quiz)
				.ThenInclude(quiz => quiz.Questions)
			.Include(attempt => attempt.Items)
				.ThenInclude(item => item.Question)
			.Include(attempt => attempt.Items)
				.ThenInclude(item => item.SrsItem)
			.FirstOrDefaultAsync(attempt => attempt.Id == attemptId && attempt.UserId == userId, cancellationToken);
	}

	public async Task<IReadOnlyDictionary<Guid, QuizItemAnswerHistory>> GetAnswerHistoryByItemAsync(
		UserId userId,
		IReadOnlyCollection<Guid> itemIds,
		QuizAttemptId excludeAttemptId,
		CancellationToken cancellationToken = default)
	{
		if (itemIds.Count == 0)
			return new Dictionary<Guid, QuizItemAnswerHistory>();

		var typedItemIds = itemIds.Distinct().Select(SrsItemId.From).ToArray();
		var answerRows = await _context.QuizAttemptItems
			.AsNoTracking()
			.Where(item =>
				typedItemIds.Contains(item.SrsItemId) &&
				item.QuizAttempt.UserId == userId &&
				item.QuizAttempt.Id != excludeAttemptId &&
				item.QuizAttempt.Status == QuizAttemptStatus.Completed &&
				item.QuizAttempt.CompletedAt != null &&
				item.AnsweredAt != null &&
				item.IsCorrect != null)
			.Select(item => new
			{
				ItemId = item.SrsItemId.Value,
				IsCorrect = item.IsCorrect!.Value
			})
			.ToListAsync(cancellationToken);

		return answerRows
			.GroupBy(item => item.ItemId)
			.ToDictionary(
				group => group.Key,
				group => new QuizItemAnswerHistory(
					group.Count(),
					group.Count(item => item.IsCorrect),
					group.Count(item => !item.IsCorrect)));
	}

	public async Task<IReadOnlyDictionary<Guid, IReadOnlyList<QuizItemFeedbackHistory>>> GetFeedbackHistoryByItemAsync(
		UserId userId,
		IReadOnlyCollection<Guid> itemIds,
		QuizAttemptId excludeAttemptId,
		int maxEntriesPerItem = 3,
		CancellationToken cancellationToken = default)
	{
		if (itemIds.Count == 0 || maxEntriesPerItem <= 0)
			return new Dictionary<Guid, IReadOnlyList<QuizItemFeedbackHistory>>();

		var typedItemIds = itemIds.Distinct().Select(SrsItemId.From).ToArray();
		var history = await _context.QuizAttemptItems
			.AsNoTracking()
			.Where(item =>
				typedItemIds.Contains(item.SrsItemId) &&
				item.QuizAttempt.UserId == userId &&
				item.QuizAttempt.Id != excludeAttemptId &&
				item.QuizAttempt.Status == QuizAttemptStatus.Completed &&
				item.QuizAttempt.CompletedAt != null &&
				item.AnsweredAt != null &&
				item.IsCorrect != null)
			.OrderByDescending(item => item.AnsweredAt)
			.Select(item => new
			{
				ItemId = item.SrsItemId.Value,
				Item = new QuizItemFeedbackHistory(
					item.Id.Value,
					item.AnsweredAt!.Value,
					item.IsCorrect!.Value,
					item.SelectedOptionId,
					item.Question.ChoicesJson,
					item.Question.CorrectOptionId,
					item.Feedback)
			})
			.ToListAsync(cancellationToken);

		return history
			.GroupBy(entry => entry.ItemId)
			.ToDictionary(
				group => group.Key,
				group => (IReadOnlyList<QuizItemFeedbackHistory>)group
					.Take(maxEntriesPerItem)
					.Select(entry => entry.Item)
					.ToArray());
	}
}
