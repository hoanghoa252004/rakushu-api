using Rakushu.Domain.Common.Contract;

namespace Rakushu.Domain.Entities.Srs.Quiz;

public interface IQuizRepository : IBaseRepository<Quiz, QuizId>
{
	Task<Quiz?> GetByIdWithQuestionsAsync(QuizId id, CancellationToken cancellationToken = default);
}
