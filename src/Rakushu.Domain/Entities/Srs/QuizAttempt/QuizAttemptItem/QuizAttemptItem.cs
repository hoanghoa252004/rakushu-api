using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Srs.Quiz.Question;

namespace Rakushu.Domain.Entities.Srs.QuizAttempt.QuizAttemptItem;

public sealed class QuizAttemptItem : Entity<QuizAttemptItemId>
{
	public QuizAttemptId QuizAttemptId { get; private set; } = null!;
	public QuestionId QuestionId { get; private set; } = null!;
	public int Position { get; private set; }
	public string? SelectedOptionId { get; private set; }
	public bool? IsCorrect { get; private set; }
	public int ElapsedMs { get; private set; }
	public string? Feedback { get; private set; }
	public DateTimeOffset? AnsweredAt { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	public QuizAttempt QuizAttempt { get; private set; } = null!;
	public Question Question { get; private set; } = null!;

	private QuizAttemptItem() { }
	private QuizAttemptItem(QuizAttemptItemId id) : base(id) { }

	public static Result<QuizAttemptItem> Create(
		QuestionId questionId,
		int position,
		DateTimeOffset createdAt)
	{
		var validation = Validate(questionId, position);
		if (validation.IsFailure)
			return Result.Failure<QuizAttemptItem>(validation.Error);

		return Result.Success(new QuizAttemptItem(QuizAttemptItemId.Create())
		{
			QuestionId = questionId,
			Position = position,
			CreatedAt = createdAt
		});
	}

	internal void AssignToAttempt(QuizAttemptId attemptId, int position)
	{
		QuizAttemptId = attemptId;
		Position = position;
	}

	public Result RecordAnswer(
		string? selectedOptionId,
		bool isCorrect,
		int elapsedMs,
		string? feedback,
		DateTimeOffset answeredAt)
	{
		SelectedOptionId = string.IsNullOrWhiteSpace(selectedOptionId) ? null : selectedOptionId.Trim();
		IsCorrect = isCorrect;
		ElapsedMs = Math.Max(0, elapsedMs);
		Feedback = string.IsNullOrWhiteSpace(feedback) ? null : feedback.Trim();
		AnsweredAt = answeredAt;

		return Result.Success();
	}

	public void UpdateFeedback(string? feedback)
	{
		Feedback = string.IsNullOrWhiteSpace(feedback) ? null : feedback.Trim();
	}

	private static Result Validate(QuestionId questionId, int position)
	{
		if (questionId is null || questionId.Value == Guid.Empty)
			return Result.Failure(QuizAttemptItemErrors.InvalidQuestionId);

		if (position < 1)
			return Result.Failure(QuizAttemptItemErrors.InvalidPosition);

		return Result.Success();
	}
}
