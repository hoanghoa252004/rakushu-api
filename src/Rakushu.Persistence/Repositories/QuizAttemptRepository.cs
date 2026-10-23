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
			.Include(attempt => attempt.Items)
				.ThenInclude(item => item.SrsCard)
					.ThenInclude(card => card.State)
			.Include(attempt => attempt.Items)
				.ThenInclude(item => item.SrsCard)
					.ThenInclude(card => card.SrsNote)
						.ThenInclude(note => note.LearningUnit)
							.ThenInclude(unit => unit.KnowledgeMeaning)
								.ThenInclude(meaning => meaning.KnowledgeUnit)
			.FirstOrDefaultAsync(attempt => attempt.Id == attemptId && attempt.UserId == userId, cancellationToken);
	}

	public async Task<IReadOnlyDictionary<Guid, QuizCardAnswerHistory>> GetAnswerHistoryByCardAsync(
		UserId userId,
		IReadOnlyCollection<Guid> cardIds,
		QuizAttemptId excludeAttemptId,
		CancellationToken cancellationToken = default)
	{
		if (cardIds.Count == 0)
			return new Dictionary<Guid, QuizCardAnswerHistory>();

		var typedCardIds = cardIds.Distinct().Select(SrsCardId.From).ToArray();
		// Keep the database query flat: grouping on SrsCardId.Value (a value-object
		// member) is not translatable by the configured EF Core provider.
		var answerRows = await _context.QuizAttemptItems
			.AsNoTracking()
			.Where(item =>
				typedCardIds.Contains(item.SrsCardId) &&
				item.QuizAttempt.UserId == userId &&
				item.QuizAttempt.Id != excludeAttemptId &&
				item.QuizAttempt.AttemptMode == "SrsReview" &&
				item.QuizAttempt.Status == QuizAttemptStatus.Completed &&
				item.QuizAttempt.CompletedAt != null &&
				item.QuizAttempt.SourceAttemptId == null &&
				item.AnsweredAt != null &&
				item.IsCorrect != null)
			.Select(item => new
			{
				CardId = item.SrsCardId.Value,
				IsCorrect = item.IsCorrect!.Value
			})
			.ToListAsync(cancellationToken);

		return answerRows
			.GroupBy(item => item.CardId)
			.ToDictionary(
				group => group.Key,
				group => new QuizCardAnswerHistory(
					group.Count(),
					group.Count(item => item.IsCorrect),
					group.Count(item => !item.IsCorrect)));
	}

	public async Task<IReadOnlyDictionary<Guid, IReadOnlyList<QuizCardFeedbackHistory>>> GetFeedbackHistoryByCardAsync(
		UserId userId,
		IReadOnlyCollection<Guid> cardIds,
		QuizAttemptId excludeAttemptId,
		int maxEntriesPerCard = 3,
		CancellationToken cancellationToken = default)
	{
		if (cardIds.Count == 0 || maxEntriesPerCard <= 0)
			return new Dictionary<Guid, IReadOnlyList<QuizCardFeedbackHistory>>();

		var typedCardIds = cardIds.Distinct().Select(SrsCardId.From).ToArray();
		var history = await _context.QuizAttemptItems
			.AsNoTracking()
			.Where(item =>
				typedCardIds.Contains(item.SrsCardId) &&
				item.QuizAttempt.UserId == userId &&
				item.QuizAttempt.Id != excludeAttemptId &&
				item.QuizAttempt.AttemptMode == "SrsReview" &&
				item.QuizAttempt.Status == QuizAttemptStatus.Completed &&
				item.QuizAttempt.CompletedAt != null &&
				item.QuizAttempt.SourceAttemptId == null &&
				item.AnsweredAt != null &&
				item.IsCorrect != null)
			.OrderByDescending(item => item.AnsweredAt)
			.Select(item => new
			{
				CardId = item.SrsCardId.Value,
				Item = new QuizCardFeedbackHistory(
					item.Id.Value,
					item.AnsweredAt!.Value,
					item.IsCorrect!.Value,
					item.SelectedAnswer,
					item.ChoicesSnapshot,
					item.CorrectAnswerSnapshot,
					item.FeedbackSnapshot)
			})
			.ToListAsync(cancellationToken);

		return history
			.GroupBy(entry => entry.CardId)
			.ToDictionary(
				group => group.Key,
				group => (IReadOnlyList<QuizCardFeedbackHistory>)group
					.Take(maxEntriesPerCard)
					.Select(entry => entry.Item)
					.ToArray());
	}
}
