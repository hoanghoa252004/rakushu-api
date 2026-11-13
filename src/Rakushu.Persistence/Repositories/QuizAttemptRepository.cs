using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.QuizAttempt;
using Rakushu.Domain.Entities.Srs.QuizAttempt.QuizAttemptItem;
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
						.ThenInclude(bq => bq.QuestionType)
			.Include(qa => qa.Quiz)
				.ThenInclude(q => q.Questions)
					.ThenInclude(qn => qn.SavedItem)
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
					.ThenInclude(qn => qn.BankQuestion)
						.ThenInclude(bq => bq.QuestionType)
			.FirstOrDefaultAsync(qa => qa.UserId == userId && qa.Status == QuizAttemptStatus.InProgress, cancellationToken);
	}

	public void RemoveItems(IEnumerable<QuizAttemptItem> items)
	{
		_context.QuizAttemptItems.RemoveRange(items);
	}

	public async Task<(IReadOnlyList<QuizAttempt> Items, int TotalCount)> GetAttemptsByUserAsync(
		UserId userId,
		QuizAttemptStatus? status,
		int pageNumber,
		int pageSize,
		CancellationToken cancellationToken = default)
	{
		var query = _context.QuizAttempts
			.Where(qa => qa.UserId == userId);

		if (status.HasValue)
		{
			query = query.Where(qa => qa.Status == status.Value);
		}

		var totalCount = await query.CountAsync(cancellationToken);

		var items = await query
			.OrderByDescending(qa => qa.CreatedAt)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);

		return (items, totalCount);
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
					item.SelectedAnswer,
					item.Question.Answers,
					item.Question.CorrectAnswer,
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

	public async Task<IReadOnlyList<QuizAttemptId>> GetPendingEvaluationAttemptIdsAsync(
		int maxCount,
		CancellationToken cancellationToken = default)
	{
		return await _context.QuizAttempts
			.AsNoTracking()
			.Where(a => a.Status == QuizAttemptStatus.Completed && !a.IsEvaluated)
			.OrderBy(a => a.CompletedAt)
			.Take(maxCount)
			.Select(a => a.Id)
			.ToListAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<QuizAttempt>> GetStaleAttemptsAsync(
		DateTimeOffset now,
		int maxCount,
		CancellationToken cancellationToken = default)
	{
		return await _context.QuizAttempts
			.Include(qa => qa.Items)
			.Include(qa => qa.Quiz)
				.ThenInclude(q => q.Questions)
			.Where(a => a.Status == QuizAttemptStatus.InProgress && a.ExpiresAt < now)
			.OrderBy(a => a.ExpiresAt)
			.Take(maxCount)
			.ToListAsync(cancellationToken);
	}
}
