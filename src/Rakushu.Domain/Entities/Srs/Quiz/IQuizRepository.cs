using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Domain.Entities.Srs;

public interface IQuizRepository : IBaseRepository<Quiz, QuizId>
{
	Task<Quiz?> GetForUserAsync(QuizId quizId, UserId userId, CancellationToken cancellationToken = default);
}
