using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Domain.Entities.Srs;

public interface IQuizAttemptRepository : IBaseRepository<QuizAttempt, QuizAttemptId>
{
    Task<QuizAttempt?> GetForUserAsync(QuizAttemptId attemptId, UserId userId, CancellationToken cancellationToken = default);
    Task<IReadOnlySet<Guid>> GetCardsWithPreviousAnswersAsync(
        UserId userId,
        IReadOnlyCollection<Guid> cardIds,
        QuizAttemptId excludeAttemptId,
        CancellationToken cancellationToken = default);
}
