using Rakushu.Domain.Common.Results;

namespace Rakushu.Domain.Entities.Srs;

public sealed class QuizAttemptItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid QuizAttemptId { get; private set; }
    public Guid SrsCardId { get; private set; }
    public int Position { get; private set; }
    public string QuestionType { get; private set; } = "WORD_TO_MEANING_MCQ";
    public string QuestionSnapshot { get; private set; } = "{}";
    public string? ChoicesSnapshot { get; private set; }
    public string CorrectAnswerSnapshot { get; private set; } = "{}";
    public string? SelectedAnswer { get; private set; }
    public bool? IsCorrect { get; private set; }
    public string? FeedbackSnapshot { get; private set; }
    public DateTimeOffset? AnsweredAt { get; private set; }
    public long? ElapsedMs { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public QuizAttempt QuizAttempt { get; private set; } = null!;
    public SrsCard SrsCard { get; private set; } = null!;

    private QuizAttemptItem() { }

    public static Result<QuizAttemptItem> Create(
        Guid cardId,
        int position,
        string questionType,
        string questionSnapshot,
        string correctAnswerSnapshot,
        DateTimeOffset createdAt,
        string? choicesSnapshot = null,
        string? feedbackSnapshot = null)
    {
        if (cardId == Guid.Empty || position < 1)
            return Result.Failure<QuizAttemptItem>(QuizAttemptErrors.InvalidCardId);
        if (string.IsNullOrWhiteSpace(questionType) ||
            string.IsNullOrWhiteSpace(questionSnapshot) ||
            string.IsNullOrWhiteSpace(correctAnswerSnapshot))
            return Result.Failure<QuizAttemptItem>(QuizAttemptErrors.InvalidQuizAnswer);

        return Result.Success(new QuizAttemptItem
        {
            SrsCardId = cardId,
            Position = position,
            QuestionType = questionType,
            QuestionSnapshot = questionSnapshot,
            CorrectAnswerSnapshot = correctAnswerSnapshot,
            ChoicesSnapshot = choicesSnapshot,
            FeedbackSnapshot = feedbackSnapshot,
            CreatedAt = createdAt
        });
    }

    internal Result<QuizAttemptItem> CreateRetryCopy(DateTimeOffset createdAt) =>
        Create(SrsCardId, Position, QuestionType, QuestionSnapshot, CorrectAnswerSnapshot,
            createdAt, ChoicesSnapshot, FeedbackSnapshot);

    internal void AssignToAttempt(Guid attemptId, int position)
    {
        if (attemptId == Guid.Empty)
            throw new ArgumentException("A valid quiz attempt ID is required.", nameof(attemptId));
        QuizAttemptId = attemptId;
        Position = position;
    }

    internal Result RecordAnswer(string selectedAnswer, bool isCorrect, DateTimeOffset answeredAt, long? elapsedMs)
    {
        if (AnsweredAt is not null)
            return Result.Failure(QuizAttemptErrors.QuizItemAlreadyAnswered);
        if (string.IsNullOrWhiteSpace(selectedAnswer))
            return Result.Failure(QuizAttemptErrors.InvalidQuizAnswer);
        if (elapsedMs is < 0 || answeredAt < CreatedAt)
            return Result.Failure(QuizAttemptErrors.InvalidReviewTiming);

        SelectedAnswer = selectedAnswer;
        IsCorrect = isCorrect;
        AnsweredAt = answeredAt;
        ElapsedMs = elapsedMs;
        return Result.Success();
    }
}