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
	Task<IReadOnlyDictionary<Guid, IReadOnlyList<QuizCardFeedbackHistory>>> GetFeedbackHistoryByCardAsync(
		UserId userId,
		IReadOnlyCollection<Guid> cardIds,
		QuizAttemptId excludeAttemptId,
		int maxEntriesPerCard = 3,
		CancellationToken cancellationToken = default);
}

public sealed record QuizCardAnswerHistory(
	int TotalAttempts,
	int CorrectAttempts,
	int IncorrectAttempts);

public sealed record QuizCardFeedbackHistory(
	Guid ItemId,
	DateTimeOffset AnsweredAt,
	bool IsCorrect,
	string? SelectedAnswer,
	string? ChoicesSnapshot,
	string CorrectAnswerSnapshot,
	string? FeedbackSnapshot);
