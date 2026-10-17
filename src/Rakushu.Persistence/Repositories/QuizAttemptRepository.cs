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
}