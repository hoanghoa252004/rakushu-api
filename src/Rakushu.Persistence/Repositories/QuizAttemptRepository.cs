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

	public async Task<IReadOnlySet<Guid>> GetCardsWithPreviousAnswersAsync(
		UserId userId,
		IReadOnlyCollection<Guid> cardIds,
		QuizAttemptId excludeAttemptId,
		CancellationToken cancellationToken = default)
	{
		if (cardIds.Count == 0)
			return new HashSet<Guid>();

		var typedCardIds = cardIds.Distinct().Select(SrsCardId.From).ToArray();
		var cardsWithPreviousAnswers = await _context.QuizAttemptItems
			.AsNoTracking()
			.Where(item =>
				typedCardIds.Contains(item.SrsCardId) &&
				item.QuizAttempt.UserId == userId &&
				item.QuizAttempt.Id != excludeAttemptId &&
				item.QuizAttempt.Status == QuizAttemptStatus.Completed &&
				item.QuizAttempt.CompletedAt != null &&
				item.QuizAttempt.SourceAttemptId == null &&
				item.AnsweredAt != null &&
				item.IsCorrect != null)
			.Select(item => item.SrsCardId)
			.Distinct()
			.ToListAsync(cancellationToken);

		return cardsWithPreviousAnswers.Select(cardId => cardId.Value).ToHashSet();
	}
}