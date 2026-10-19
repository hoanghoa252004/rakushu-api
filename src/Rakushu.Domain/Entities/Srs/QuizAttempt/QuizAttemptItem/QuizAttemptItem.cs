using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Domain.Entities.Srs;

public sealed class QuizAttemptItem : Entity<QuizAttemptItemId>
{
	public QuizAttemptId QuizAttemptId { get; private set; } = null!;
	public SrsCardId SrsCardId { get; private set; } = null!;
	public int Position { get; private set; }
	public string QuestionType { get; private set; } = "WORD_TO_MEANING_MCQ";
	public string QuestionSnapshot { get; private set; } = "{}";
	public string? ChoicesSnapshot { get; private set; }
	public string CorrectAnswerSnapshot { get; private set; } = "{}";
	public string? SelectedAnswer { get; private set; }
	public bool? IsCorrect { get; private set; }
	public string? FeedbackSnapshot { get; private set; }
	public DateTimeOffset? AnsweredAt { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public QuizAttempt QuizAttempt { get; private set; } = null!;
	public SrsCard SrsCard { get; private set; } = null!;

	private QuizAttemptItem() { }

	private QuizAttemptItem(QuizAttemptItemId id) : base(id) { }

	public static Result<QuizAttemptItem> Create(
		SrsCardId cardId, int position, string questionType, string questionSnapshot,
		string correctAnswerSnapshot, DateTimeOffset createdAt, string? choicesSnapshot = null, string? feedbackSnapshot = null)
	{
		if (cardId is null || cardId.Value == Guid.Empty || position < 1)
			return Result.Failure<QuizAttemptItem>(QuizAttemptErrors.InvalidCardId);
		if (string.IsNullOrWhiteSpace(questionType) || string.IsNullOrWhiteSpace(questionSnapshot) || string.IsNullOrWhiteSpace(correctAnswerSnapshot))
			return Result.Failure<QuizAttemptItem>(QuizAttemptErrors.InvalidQuizAnswer);

		var item = new QuizAttemptItem(QuizAttemptItemId.Create())
		{
			SrsCardId = cardId,
			Position = position,
			QuestionType = questionType,
			QuestionSnapshot = questionSnapshot,
			CorrectAnswerSnapshot = correctAnswerSnapshot,
			ChoicesSnapshot = choicesSnapshot,
			FeedbackSnapshot = feedbackSnapshot,
			CreatedAt = createdAt
		};
		return Result.Success(item);
	}

	internal Result<QuizAttemptItem> CreateRetryCopy(DateTimeOffset createdAt) =>
		Create(SrsCardId, Position, QuestionType, QuestionSnapshot, CorrectAnswerSnapshot, createdAt, ChoicesSnapshot, FeedbackSnapshot);

	internal void AssignToAttempt(QuizAttemptId attemptId, int position)
	{
		if (attemptId is null || attemptId.Value == Guid.Empty)
			throw new ArgumentException("A valid quiz attempt ID is required.", nameof(attemptId));
		QuizAttemptId = attemptId;
		Position = position;
	}

	public Result UpdateFeedbackSnapshot(string feedbackSnapshot)
	{
		if (string.IsNullOrWhiteSpace(feedbackSnapshot))
			return Result.Failure(QuizAttemptErrors.InvalidQuizAnswer);

		FeedbackSnapshot = feedbackSnapshot;
		return Result.Success();
	}

	internal Result RecordAnswer(string selectedAnswer, bool isCorrect, DateTimeOffset answeredAt)
	{
		if (AnsweredAt is not null)
			return Result.Failure(QuizAttemptErrors.QuizItemAlreadyAnswered);
		if (string.IsNullOrWhiteSpace(selectedAnswer))
			return Result.Failure(QuizAttemptErrors.InvalidQuizAnswer);
		if (answeredAt < CreatedAt)
			return Result.Failure(QuizAttemptErrors.InvalidReviewTiming);

		SelectedAnswer = selectedAnswer;
		IsCorrect = isCorrect;
		AnsweredAt = answeredAt;
		return Result.Success();
	}
}
