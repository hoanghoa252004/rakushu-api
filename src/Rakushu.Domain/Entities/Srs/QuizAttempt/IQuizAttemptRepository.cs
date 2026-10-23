using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Domain.Entities.Srs;

public interface IQuizAttemptRepository : IBaseRepository<QuizAttempt, QuizAttemptId>
{
	Task<QuizAttempt?> GetForUserAsync(QuizAttemptId attemptId, UserId userId, CancellationToken cancellationToken = default);
	Task<IReadOnlyDictionary<Guid, QuizCardAnswerHistory>> GetAnswerHistoryByCardAsync(
		UserId userId,
		IReadOnlyCollection<Guid> cardIds,
		QuizAttemptId excludeAttemptId,
		CancellationToken cancellationToken = default);
}

public sealed record QuizCardAnswerHistory(
	int TotalAttempts,
	int CorrectAttempts,
	int IncorrectAttempts);
