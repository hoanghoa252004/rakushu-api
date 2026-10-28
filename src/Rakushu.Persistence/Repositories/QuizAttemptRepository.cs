using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.QuizAttempt;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Persistence.Repositories;

public sealed class QuizAttemptRepository : BaseRepository<QuizAttempt, QuizAttemptId>, IQuizAttemptRepository
{
	public QuizAttemptRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<QuizAttempt?> GetByIdWithItemsAndQuizAsync(
		QuizAttemptId id,
		CancellationToken cancellationToken = default)
	{
		return await _context.QuizAttempts
			.Include(qa => qa.Items)
			.Include(qa => qa.Quiz)
				.ThenInclude(q => q.Questions)
					.ThenInclude(qn => qn.BankQuestion)
			.SingleOrDefaultAsync(qa => qa.Id == id, cancellationToken);
	}

	public async Task<QuizAttempt?> GetActiveAttemptForUserAsync(
		UserId userId,
		CancellationToken cancellationToken = default)
	{
		return await _context.QuizAttempts
			.Include(qa => qa.Items)
			.Include(qa => qa.Quiz)
				.ThenInclude(q => q.Questions)
			.FirstOrDefaultAsync(qa => qa.UserId == userId && qa.Status == QuizAttemptStatus.InProgress, cancellationToken);
	}

	public async Task<IReadOnlyList<QuizAttempt>> GetUserAttemptsAsync(
		UserId userId,
		int page,
		int pageSize,
		CancellationToken cancellationToken = default)
	{
		return await _context.QuizAttempts
			.Where(qa => qa.UserId == userId)
			.OrderByDescending(qa => qa.StartedAt)
			.Skip((page - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyDictionary<Guid, QuizItemAnswerHistory>> GetAnswerHistoryBySavedItemAsync(
		UserId userId,
		IReadOnlyCollection<SavedItemId> savedItemIds,
		QuizAttemptId excludeAttemptId,
		CancellationToken cancellationToken = default)
	{
		if (savedItemIds.Count == 0)
			return new Dictionary<Guid, QuizItemAnswerHistory>();

		var typedItemIds = savedItemIds.Distinct().ToArray();
		var answerRows = await _context.QuizAttemptItems
			.AsNoTracking()
			.Where(item =>
				typedItemIds.Contains(item.Question.SavedItemId) &&
				item.QuizAttempt.UserId == userId &&
				item.QuizAttempt.Id != excludeAttemptId &&
				item.QuizAttempt.Status == QuizAttemptStatus.Completed &&
				item.QuizAttempt.CompletedAt != null &&
				item.AnsweredAt != null &&
				item.IsCorrect != null)
			.Select(item => new
			{
				SavedItemId = item.Question.SavedItemId.Value,
				IsCorrect = item.IsCorrect!.Value
			})
			.ToListAsync(cancellationToken);

		return answerRows
			.GroupBy(item => item.SavedItemId)
			.ToDictionary(
				group => group.Key,
				group => new QuizItemAnswerHistory(
					group.Count(),
					group.Count(item => item.IsCorrect),
					group.Count(item => !item.IsCorrect)));
	}

	public async Task<IReadOnlyDictionary<Guid, IReadOnlyList<QuizItemFeedbackHistory>>> GetFeedbackHistoryBySavedItemAsync(
		UserId userId,
		IReadOnlyCollection<SavedItemId> savedItemIds,
		QuizAttemptId excludeAttemptId,
		int maxEntriesPerItem = 3,
		CancellationToken cancellationToken = default)
	{
		if (savedItemIds.Count == 0 || maxEntriesPerItem <= 0)
			return new Dictionary<Guid, IReadOnlyList<QuizItemFeedbackHistory>>();

		var typedItemIds = savedItemIds.Distinct().ToArray();
		var history = await _context.QuizAttemptItems
			.AsNoTracking()
			.Where(item =>
				typedItemIds.Contains(item.Question.SavedItemId) &&
				item.QuizAttempt.UserId == userId &&
				item.QuizAttempt.Id != excludeAttemptId &&
				item.QuizAttempt.Status == QuizAttemptStatus.Completed &&
				item.QuizAttempt.CompletedAt != null &&
				item.AnsweredAt != null &&
				item.IsCorrect != null)
			.OrderByDescending(item => item.AnsweredAt)
			.Select(item => new
			{
				SavedItemId = item.Question.SavedItemId.Value,
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
			.GroupBy(entry => entry.SavedItemId)
			.ToDictionary(
				group => group.Key,
				group => (IReadOnlyList<QuizItemFeedbackHistory>)group
					.Take(maxEntriesPerItem)
					.Select(entry => entry.Item)
					.ToArray());
	}
}
