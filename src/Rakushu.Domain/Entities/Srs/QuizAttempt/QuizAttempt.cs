using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Domain.Entities.Srs;

public sealed class QuizAttempt : AggregateRoot<QuizAttemptId>
{
	// MAIN PROPERTIES----------
	public UserId UserId { get; private set; } = null!;
	public QuizId QuizId { get; private set; } = null!;
	public QuizAttemptStatus Status { get; private set; } = QuizAttemptStatus.InProgress;
	public int TotalItems { get; private set; }
	public int AnsweredItems { get; private set; }
	public int CorrectItems { get; private set; }
	public decimal? Score { get; private set; }
	public DateTimeOffset StartedAt { get; private set; }
	public DateTimeOffset? CompletedAt { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES----------
	public User User { get; private set; } = null!;
	public Quiz Quiz { get; private set; } = null!;

	private readonly List<QuizAttemptItem> _items = [];
	public IReadOnlyCollection<QuizAttemptItem> Items => _items.AsReadOnly();

	// CONSTRUCTORS & FACTORY METHODS----------
	private QuizAttempt() { }
	private QuizAttempt(QuizAttemptId id) : base(id) { }

	public static Result<QuizAttempt> Create(UserId userId, QuizId quizId, DateTimeOffset now)
	{
		if (userId is null)
			return Result.Failure<QuizAttempt>(QuizAttemptErrors.InvalidUserId);
		if (quizId is null)
			return Result.Failure<QuizAttempt>(QuizErrors.NotFound);

		return Result.Success(new QuizAttempt(QuizAttemptId.Create())
		{
			UserId = userId,
			QuizId = quizId,
			Status = QuizAttemptStatus.InProgress,
			StartedAt = now,
			CreatedAt = now,
			UpdatedAt = now
		});
	}

	public Result AddItem(QuizAttemptItem item)
	{
		if (Status != QuizAttemptStatus.InProgress)
			return Result.Failure(QuizAttemptErrors.QuizNotInProgress);
		if (item is null || item.SrsItemId.Value == Guid.Empty)
			return Result.Failure(QuizAttemptErrors.InvalidItemId);
		if (_items.Any(x => x.Position == item.Position) || _items.Any(x => x.QuestionId == item.QuestionId))
			return Result.Failure(QuizAttemptErrors.DuplicateQuizItem);

		item.AssignToAttempt(Id, _items.Count + 1);
		_items.Add(item);
		TotalItems = _items.Count;
		UpdatedAt = item.CreatedAt;
		return Result.Success();
	}

	public Result RecordAnswer(
		Guid itemId,
		string? selectedOptionId,
		bool isCorrect,
		int elapsedMs,
		int quality,
		string? feedback,
		DateTimeOffset answeredAt)
	{
		if (Status != QuizAttemptStatus.InProgress)
			return Result.Failure(QuizAttemptErrors.QuizNotInProgress);

		var item = _items.SingleOrDefault(x => x.Id.Value == itemId);
		if (item is null)
			return Result.Failure(QuizAttemptErrors.QuizItemNotFound);

		var answerResult = item.RecordAnswer(selectedOptionId, isCorrect, elapsedMs, quality, feedback, answeredAt);
		if (answerResult.IsFailure)
			return answerResult;

		AnsweredItems = _items.Count(x => x.AnsweredAt is not null);
		CorrectItems = _items.Count(x => x.IsCorrect == true);
		UpdatedAt = answeredAt;
		return Result.Success();
	}

	public Result Complete(DateTimeOffset completedAt)
	{
		if (Status != QuizAttemptStatus.InProgress)
			return Result.Failure(QuizAttemptErrors.QuizNotInProgress);
		if (TotalItems == 0 || AnsweredItems != TotalItems)
			return Result.Failure(QuizAttemptErrors.QuizHasUnansweredItems);

		Status = QuizAttemptStatus.Completed;
		CompletedAt = completedAt;
		Score = Math.Round((decimal)CorrectItems * 100m / TotalItems, 2);
		UpdatedAt = completedAt;
		return Result.Success();
	}

	public Result Abandon(DateTimeOffset abandonedAt)
	{
		if (Status != QuizAttemptStatus.InProgress)
			return Result.Failure(QuizAttemptErrors.QuizNotInProgress);

		Status = QuizAttemptStatus.Abandoned;
		UpdatedAt = abandonedAt;
		return Result.Success();
	}
}
