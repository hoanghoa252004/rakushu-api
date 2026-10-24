using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs;

namespace Rakushu.Persistence.Repositories;

public sealed class QuizRepository : BaseRepository<Quiz, QuizId>, IQuizRepository
{
	public QuizRepository(RakushuDbContext context) : base(context) { }

	public Task<Quiz?> GetForUserAsync(QuizId quizId, UserId userId, CancellationToken cancellationToken = default)
	{
		return _context.Quizzes
			.Include(quiz => quiz.Questions)
			.FirstOrDefaultAsync(quiz => quiz.Id == quizId && quiz.UserId == userId, cancellationToken);
	}
}
