using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Srs.Quiz.Question;

namespace Rakushu.Domain.Entities.Srs.QuizAttempt.QuizAttemptItem;

public sealed class QuizAttemptItem : Entity<QuizAttemptItemId>
{
	public QuizAttemptId QuizAttemptId { get; private set; } = null!;
	public QuestionId QuestionId { get; private set; } = null!;
	public string? SelectedAnswer { get; private set; }
	public bool? IsCorrect { get; private set; }
	public int TimeSpentMs { get; private set; }
	public string? Feedback { get; private set; }
	public DateTimeOffset? AnsweredAt { get; private set; }

	// NAVIGATION PROPERTIES
	public QuizAttempt QuizAttempt { get; private set; } = null!;
	public Question Question { get; private set; } = null!;

	private QuizAttemptItem() { }
	private QuizAttemptItem(QuizAttemptItemId id) : base(id) { }

	public static Result<QuizAttemptItem> Create(
		QuestionId questionId,
		string? selectedAnswer = null,
		bool? isCorrect = null,
		int timeSpentMs = 0,
		string? feedback = null,
		DateTimeOffset? answeredAt = null)
	{
		var validation = Validate(questionId);
		if (validation.IsFailure)
			return Result.Failure<QuizAttemptItem>(validation.Error);

		return Result.Success(new QuizAttemptItem(QuizAttemptItemId.Create())
		{
			QuestionId = questionId,
			SelectedAnswer = string.IsNullOrWhiteSpace(selectedAnswer) ? null : selectedAnswer.Trim(),
			IsCorrect = isCorrect,
			TimeSpentMs = Math.Max(0, timeSpentMs),
			Feedback = string.IsNullOrWhiteSpace(feedback) ? null : feedback.Trim(),
			AnsweredAt = answeredAt
		});
	}

	internal void AssignToAttempt(QuizAttemptId attemptId)
	{
		QuizAttemptId = attemptId;
	}

	public Result RecordAnswer(
		string? selectedAnswer,
		bool? isCorrect,
		int timeSpentMs,
		string? feedback,
		DateTimeOffset answeredAt)
	{
		SelectedAnswer = string.IsNullOrWhiteSpace(selectedAnswer) ? null : selectedAnswer.Trim();
		IsCorrect = isCorrect;
		TimeSpentMs = Math.Max(0, timeSpentMs);
		Feedback = string.IsNullOrWhiteSpace(feedback) ? null : feedback.Trim();
		AnsweredAt = answeredAt;

		return Result.Success();
	}

	public Result UpdateAnswer(
		string? selectedAnswer,
		bool? isCorrect,
		int timeSpentMs,
		DateTimeOffset answeredAt)
	{
		SelectedAnswer = string.IsNullOrWhiteSpace(selectedAnswer) ? null : selectedAnswer.Trim();
		IsCorrect = isCorrect;
		TimeSpentMs = Math.Max(0, timeSpentMs);
		AnsweredAt = answeredAt;

		return Result.Success();
	}

	public void SetResult(bool isCorrect)
	{
		IsCorrect = isCorrect;
	}

	public void UpdateFeedback(string? feedback)
	{
		Feedback = string.IsNullOrWhiteSpace(feedback) ? null : feedback.Trim();
	}

	private static Result Validate(QuestionId questionId)
	{
		if (questionId is null || questionId.Value == Guid.Empty)
			return Result.Failure(QuizAttemptItemErrors.InvalidQuestionId);

		return Result.Success();
	}
}
