using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.Quiz;
using Rakushu.Domain.Entities.Srs.QuizAttempt.QuizAttemptItem;

namespace Rakushu.Domain.Entities.Srs.QuizAttempt;

public sealed class QuizAttempt : AggregateRoot<QuizAttemptId>
{
	private static readonly TimeSpan DefaultDuration = TimeSpan.FromHours(1);

	public UserId UserId { get; private set; } = null!;
	public QuizId QuizId { get; private set; } = null!;
	public QuizAttemptStatus Status { get; private set; }
	public QuizAttemptMode Mode { get; private set; }
	public int TotalItems { get; private set; }
	public int AnsweredItems { get; private set; }
	public int CorrectItems { get; private set; }
	public decimal? Score { get; private set; }
	public DateTimeOffset ExpiresAt { get; private set; }
	public bool IsEvaluated { get; private set; }
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
		int totalItems,
		QuizAttemptMode mode = QuizAttemptMode.SrsReview,
		TimeSpan? duration = null)
	{
		var validation = Validate(userId, quizId);
		if (validation.IsFailure)
			return Result.Failure<QuizAttempt>(validation.Error);

		var effectiveDuration = duration.HasValue && duration.Value > TimeSpan.Zero
			? duration.Value
			: DefaultDuration;

		return Result.Success(new QuizAttempt(QuizAttemptId.Create())
		{
			UserId = userId,
			QuizId = quizId,
			Mode = mode,
			TotalItems = Math.Max(0, totalItems),
			AnsweredItems = 0,
			CorrectItems = 0,
			Score = null,
			Status = QuizAttemptStatus.InProgress,
			IsEvaluated = false,
			ExpiresAt = now.Add(effectiveDuration),
			CreatedAt = now,
			UpdatedAt = now
		});
	}

	public static Result<QuizAttempt> Create(
		UserId userId,
		QuizId quizId,
		DateTimeOffset now,
		QuizAttemptMode mode,
		TimeSpan? duration = null) =>
		Create(userId, quizId, now, 0, mode, duration);

	public void SetTotalItems(int totalItems)
	{
		TotalItems = Math.Max(0, totalItems);
	}

	public Result AddItem(QuizAttemptItem.QuizAttemptItem item)
	{
		if (item is null)
			return Result.Failure(QuizAttemptItemErrors.InvalidQuestionId);

		if (_items.Any(i => i.QuestionId == item.QuestionId))
			return Result.Failure(QuizAttemptErrors.QuestionAlreadyAnswered);

		item.AssignToAttempt(Id);
		_items.Add(item);

		if (item.AnsweredAt.HasValue)
		{
			AnsweredItems++;
			if (item.IsCorrect == true)
				CorrectItems++;
		}

		return Result.Success();
	}

	public IReadOnlyList<QuizAttemptItem.QuizAttemptItem> PruneUnansweredItems()
	{
		return [];
	}

	public IReadOnlyList<QuizAttemptItem.QuizAttemptItem> ExpireAndPrune(DateTimeOffset now)
	{
		var unrated = PruneUnansweredItems();
		Expire(now);
		return unrated;
	}

	public Result Complete(DateTimeOffset now)
	{
		if (Status == QuizAttemptStatus.Completed)
			return Result.Failure(QuizAttemptErrors.AlreadyCompleted);

		if (Status == QuizAttemptStatus.Canceled)
			return Result.Failure(QuizAttemptErrors.AlreadyCanceled);

		if (now > ExpiresAt)
		{
			Status = QuizAttemptStatus.Expired;
			UpdatedAt = now;
			return Result.Failure(QuizAttemptErrors.AttemptExpired);
		}

		if (!HasAnyAnsweredItems)
			return Result.Failure(QuizAttemptErrors.NoItemsAnswered);

		Status = QuizAttemptStatus.Completed;
		CompletedAt = now;
		UpdatedAt = now;

		AnsweredItems = _items.Count(i => i.AnsweredAt.HasValue);
		CorrectItems = _items.Count(i => i.IsCorrect == true);
		Score = TotalItems > 0 ? Math.Round((decimal)CorrectItems / TotalItems * 100, 2) : 0;

		return Result.Success();
	}

	public void Expire(DateTimeOffset now)
	{
		Status = QuizAttemptStatus.Expired;
		CompletedAt = now;
		UpdatedAt = now;

		AnsweredItems = _items.Count(i => i.AnsweredAt.HasValue);
		CorrectItems = _items.Count(i => i.IsCorrect == true);
		Score = TotalItems > 0 ? Math.Round((decimal)CorrectItems / TotalItems * 100, 2) : 0;
	}

	public Result Cancel(DateTimeOffset now)
	{
		if (Status == QuizAttemptStatus.Completed)
			return Result.Failure(QuizAttemptErrors.AlreadyCompleted);

		if (Status == QuizAttemptStatus.Canceled)
			return Result.Failure(QuizAttemptErrors.AlreadyCanceled);

		if (Status != QuizAttemptStatus.InProgress)
			return Result.Failure(QuizAttemptErrors.NotInProgress);

		Status = QuizAttemptStatus.Canceled;
		CompletedAt = now;
		UpdatedAt = now;

		AnsweredItems = _items.Count(i => i.AnsweredAt.HasValue);
		CorrectItems = _items.Count(i => i.IsCorrect == true);
		Score = null;

		return Result.Success();
	}

	public void MarkEvaluated(DateTimeOffset now)
	{
		IsEvaluated = true;
		UpdatedAt = now;
	}

	public void RecalculateProgress(DateTimeOffset now)
	{
		AnsweredItems = _items.Count(i => i.AnsweredAt.HasValue);
		CorrectItems = _items.Count(i => i.IsCorrect == true);
		UpdatedAt = now;
	}

	public bool IsExpired(DateTimeOffset now) =>
		Status == QuizAttemptStatus.InProgress && now > ExpiresAt;

	private static Result Validate(UserId userId, QuizId quizId)
	{
		if (userId is null || userId.Value == Guid.Empty)
			return Result.Failure(QuizAttemptErrors.InvalidUserId);

		if (quizId is null || quizId.Value == Guid.Empty)
			return Result.Failure(QuizAttemptErrors.InvalidQuizId);

		return Result.Success();
	}

	public bool HasAnyAnsweredItems => _items.Any(i => i.AnsweredAt.HasValue);
}
