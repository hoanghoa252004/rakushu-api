using Rakushu.Domain.Common.Contract;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.SavedItem;

namespace Rakushu.Domain.Entities.Srs.QuizAttempt;

public interface IQuizAttemptRepository : IBaseRepository<QuizAttempt, QuizAttemptId>
{
	Task<QuizAttempt?> GetByIdWithItemsAndQuizAsync(QuizAttemptId id, CancellationToken cancellationToken = default);

	Task<QuizAttempt?> GetActiveAttemptForUserAsync(UserId userId, CancellationToken cancellationToken = default);

	Task<IReadOnlyList<QuizAttempt>> GetUserAttemptsAsync(
		UserId userId,
		int page,
		int pageSize,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyDictionary<Guid, QuizItemAnswerHistory>> GetAnswerHistoryBySavedItemAsync(
		UserId userId,
		IReadOnlyCollection<SavedItemId> savedItemIds,
		QuizAttemptId excludeAttemptId,
		CancellationToken cancellationToken = default);

	Task<IReadOnlyDictionary<Guid, IReadOnlyList<QuizItemFeedbackHistory>>> GetFeedbackHistoryBySavedItemAsync(
		UserId userId,
		IReadOnlyCollection<SavedItemId> savedItemIds,
		QuizAttemptId excludeAttemptId,
		int maxEntriesPerItem = 3,
		CancellationToken cancellationToken = default);
}

public sealed record QuizItemAnswerHistory(
	int TotalAttempts,
	int CorrectAttempts,
	int IncorrectAttempts);

public sealed record QuizItemFeedbackHistory(
	Guid AttemptItemId,
	DateTimeOffset AnsweredAt,
	bool IsCorrect,
	string? SelectedAnswer,
	string? ChoicesSnapshot,
	string? CorrectAnswerSnapshot,
	string? FeedbackSnapshot);
