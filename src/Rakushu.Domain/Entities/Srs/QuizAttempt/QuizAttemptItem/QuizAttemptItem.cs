using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Domain.Entities.Srs;

public sealed class QuizAttemptItem : Entity<QuizAttemptItemId>
{
	// MAIN PROPERTIES----------
	public QuizAttemptId QuizAttemptId { get; private set; } = null!;
	public QuestionId QuestionId { get; private set; } = null!;
	public SrsItemId SrsItemId { get; private set; } = null!;
	public int Position { get; private set; }
	public string? SelectedOptionId { get; private set; }
	public bool? IsCorrect { get; private set; }
	public int ElapsedMs { get; private set; }
	public int? Quality { get; private set; }
	public string? Feedback { get; private set; }
	public DateTimeOffset? AnsweredAt { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }

	// NAVIGATION PROPERTIES----------
	public QuizAttempt QuizAttempt { get; private set; } = null!;
	public Question Question { get; private set; } = null!;
	public SrsItem SrsItem { get; private set; } = null!;

	// CONSTRUCTORS & FACTORY METHODS----------
	private QuizAttemptItem() { }
	private QuizAttemptItem(QuizAttemptItemId id) : base(id) { }

	public static Result<QuizAttemptItem> Create(
		QuestionId questionId,
		SrsItemId itemId,
		int position,
		DateTimeOffset createdAt)
	{
		if (questionId is null || questionId.Value == Guid.Empty)
			return Result.Failure<QuizAttemptItem>(QuizErrors.InvalidQuestion);
		if (itemId is null || itemId.Value == Guid.Empty || position < 1)
			return Result.Failure<QuizAttemptItem>(QuizAttemptErrors.InvalidItemId);

		return Result.Success(new QuizAttemptItem(QuizAttemptItemId.Create())
		{
			QuestionId = questionId,
			SrsItemId = itemId,
			Position = position,
			CreatedAt = createdAt
		});
	}

	internal void AssignToAttempt(QuizAttemptId attemptId, int position)
	{
		if (attemptId is null || attemptId.Value == Guid.Empty)
			throw new ArgumentException("A valid quiz attempt ID is required.", nameof(attemptId));
		QuizAttemptId = attemptId;
		Position = position;
	}

	public Result RecordAnswer(
		string? selectedOptionId,
		bool isCorrect,
		int elapsedMs,
		int quality,
		string? feedback,
		DateTimeOffset answeredAt)
	{
		if (AnsweredAt is not null)
			return Result.Failure(QuizAttemptErrors.QuizItemAlreadyAnswered);
		if (answeredAt < CreatedAt)
			return Result.Failure(QuizAttemptErrors.InvalidReviewTiming);
		if (quality is < 0 or > 5)
			return Result.Failure(QuizAttemptErrors.InvalidReviewTiming);

		SelectedOptionId = string.IsNullOrWhiteSpace(selectedOptionId) ? null : selectedOptionId.Trim();
		IsCorrect = isCorrect;
		ElapsedMs = Math.Max(0, elapsedMs);
		Quality = quality;
		Feedback = string.IsNullOrWhiteSpace(feedback) ? null : feedback.Trim();
		AnsweredAt = answeredAt;
		return Result.Success();
	}

	public Result UpdateFeedback(string feedback)
	{
		if (string.IsNullOrWhiteSpace(feedback))
			return Result.Failure(QuizAttemptErrors.InvalidQuizAnswer);

		Feedback = feedback.Trim();
		return Result.Success();
	}
}
