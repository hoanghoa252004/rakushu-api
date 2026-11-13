using Microsoft.EntityFrameworkCore;
using Rakushu.Domain.Entities.Srs.Quiz;

namespace Rakushu.Persistence.Repositories;

public sealed class QuizRepository : BaseRepository<Quiz, QuizId>, IQuizRepository
{
	public QuizRepository(RakushuDbContext context) : base(context)
	{
	}

	public async Task<Quiz?> GetByIdWithQuestionsAsync(
		QuizId id,
		CancellationToken cancellationToken = default)
	{
		return await _context.Quizzes
			.Include(q => q.Questions)
				.ThenInclude(qn => qn.BankQuestion)
					.ThenInclude(bq => bq.QuestionType)
			.Include(q => q.Questions)
				.ThenInclude(qn => qn.SavedItem)
			.SingleOrDefaultAsync(q => q.Id == id, cancellationToken);
	}
}
