using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.Quiz;
using Rakushu.Domain.Entities.Srs.QuizAttempt.QuizAttemptItem;

namespace Rakushu.Domain.Entities.Srs.QuizAttempt;

public sealed class QuizAttempt : AggregateRoot<QuizAttemptId>
{
	public UserId UserId { get; private set; } = null!;
	public QuizId QuizId { get; private set; } = null!;
	public QuizAttemptStatus Status { get; private set; } = QuizAttemptStatus.InProgress;
	public QuizAttemptMode Mode { get; private set; } = QuizAttemptMode.SrsReview;
	public int TotalItems { get; private set; }
	public int AnsweredItems { get; private set; }
	public int CorrectItems { get; private set; }
	public decimal? Score { get; private set; }
	public DateTimeOffset StartedAt { get; private set; }
	public DateTimeOffset? CompletedAt { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	public User User { get; private set; } = null!;
	public Quiz.Quiz Quiz { get; private set; } = null!;

	private readonly List<QuizAttemptItem.QuizAttemptItem> _items = [];
	public IReadOnlyCollection<QuizAttemptItem.QuizAttemptItem> Items => _items.AsReadOnly();

	private QuizAttempt() { }
	private QuizAttempt(QuizAttemptId id) : base(id) { }

	public static Result<QuizAttempt> Create(
		UserId userId,
		QuizId quizId,
		DateTimeOffset now,
		QuizAttemptMode mode = QuizAttemptMode.SrsReview)
	{
		var validation = Validate(userId, quizId);
		if (validation.IsFailure)
			return Result.Failure<QuizAttempt>(validation.Error);

		return Result.Success(new QuizAttempt(QuizAttemptId.Create())
		{
			UserId = userId,
			QuizId = quizId,
			Mode = mode,
			Status = QuizAttemptStatus.InProgress,
			StartedAt = now,
			CreatedAt = now,
			UpdatedAt = now
		});
	}

	public Result AddItem(QuizAttemptItem.QuizAttemptItem item)
	{
		if (Status != QuizAttemptStatus.InProgress)
			return Result.Failure(QuizAttemptErrors.NotInProgress);

		item.AssignToAttempt(Id, _items.Count + 1);
		_items.Add(item);
		TotalItems = _items.Count;
		return Result.Success();
	}

	public Result Complete(DateTimeOffset now)
	{
		if (Status == QuizAttemptStatus.Completed)
			return Result.Failure(QuizAttemptErrors.AlreadyCompleted);

		Status = QuizAttemptStatus.Completed;
		CompletedAt = now;
		UpdatedAt = now;

		AnsweredItems = _items.Count(i => i.AnsweredAt.HasValue);
		CorrectItems = _items.Count(i => i.IsCorrect == true);
		Score = TotalItems > 0 ? Math.Round((decimal)CorrectItems / TotalItems * 100, 2) : 0;

		return Result.Success();
	}

	public void Abandon(DateTimeOffset now)
	{
		Status = QuizAttemptStatus.Abandoned;
		CompletedAt = now;
		UpdatedAt = now;
	}

	private static Result Validate(UserId userId, QuizId quizId)
	{
		if (userId is null || userId.Value == Guid.Empty)
			return Result.Failure(QuizAttemptErrors.InvalidUserId);

		if (quizId is null || quizId.Value == Guid.Empty)
			return Result.Failure(QuizAttemptErrors.InvalidQuizId);

		return Result.Success();
	}
}
