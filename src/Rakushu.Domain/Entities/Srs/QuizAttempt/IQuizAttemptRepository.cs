using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Domain.Entities.Srs;

public interface IQuizAttemptRepository : IBaseRepository<QuizAttempt, QuizAttemptId>
{
	Task<QuizAttempt?> GetForUserAsync(QuizAttemptId attemptId, UserId userId, CancellationToken cancellationToken = default);
	Task<IReadOnlyDictionary<Guid, QuizItemAnswerHistory>> GetAnswerHistoryByItemAsync(
		UserId userId,
		IReadOnlyCollection<Guid> itemIds,
		QuizAttemptId excludeAttemptId,
		CancellationToken cancellationToken = default);
	Task<IReadOnlyDictionary<Guid, IReadOnlyList<QuizItemFeedbackHistory>>> GetFeedbackHistoryByItemAsync(
		UserId userId,
		IReadOnlyCollection<Guid> itemIds,
		QuizAttemptId excludeAttemptId,
		int maxEntriesPerItem = 3,
		CancellationToken cancellationToken = default);
}

public sealed record QuizItemAnswerHistory(
	int TotalAttempts,
	int CorrectAttempts,
	int IncorrectAttempts);

public sealed record QuizItemFeedbackHistory(
	Guid ItemId,
	DateTimeOffset AnsweredAt,
	bool IsCorrect,
	string? SelectedAnswer,
	string? ChoicesSnapshot,
	string CorrectAnswerSnapshot,
	string? FeedbackSnapshot);
