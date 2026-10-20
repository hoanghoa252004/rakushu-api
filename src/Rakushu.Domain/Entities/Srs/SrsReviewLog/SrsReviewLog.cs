using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Srs;

public sealed class SrsReviewLog : Entity<SrsReviewLogId>
{
    public SrsCardId SrsCardId { get; private set; } = null!;
    public SrsSessionItemId? SrsSessionItemId { get; private set; }
    public QuizAttemptItemId? QuizAttemptItemId { get; private set; }
    public string Rating { get; private set; } = null!;
    public int Quality { get; private set; }
    public DateTimeOffset ReviewedAt { get; private set; }
    public int? ElapsedMs { get; private set; }
    public DateTimeOffset? PreviousDueAt { get; private set; }
    public DateTimeOffset? NextDueAt { get; private set; }
    public int? PreviousIntervalDays { get; private set; }
    public int? NextIntervalDays { get; private set; }
    public double? PreviousEasinessFactor { get; private set; }
    public double? NextEasinessFactor { get; private set; }
    public int? PreviousRepetitions { get; private set; }
    public int? NextRepetitions { get; private set; }
    public string SchedulerVersion { get; private set; } = "SM2-v1";
    public DateTimeOffset CreatedAt { get; private set; }
    public SrsCard SrsCard { get; private set; } = null!;

    private SrsReviewLog() { }

    private SrsReviewLog(SrsReviewLogId id) : base(id) { }

    public static SrsReviewLog Create(
        SrsCardId cardId,
        SrsSessionItemId? sessionItemId,
        QuizAttemptItemId? quizAttemptItemId,
        string rating,
        int quality,
        DateTimeOffset reviewedAt,
        int? elapsedMs,
        DateTimeOffset? previousDueAt,
        DateTimeOffset? nextDueAt,
        int? previousIntervalDays,
        int? nextIntervalDays,
        double? previousEasinessFactor,
        double? nextEasinessFactor,
        int? previousRepetitions,
        int? nextRepetitions,
        DateTimeOffset createdAt)
    {
        if (cardId is null || cardId.Value == Guid.Empty)
            throw new ArgumentException("A valid SRS card ID is required.", nameof(cardId));
        if ((sessionItemId is null) == (quizAttemptItemId is null))
            throw new ArgumentException("Exactly one review source must be specified.");
        if (string.IsNullOrWhiteSpace(rating) || !SrsReviewRating.IsValid(rating))
            throw new ArgumentException("A valid review rating is required.", nameof(rating));
        if (quality is < 0 or > 5)
            throw new ArgumentOutOfRangeException(nameof(quality), "SM-2 quality must be between 0 and 5.");
        if (elapsedMs is < 0)
            throw new ArgumentOutOfRangeException(nameof(elapsedMs));

        return new SrsReviewLog(SrsReviewLogId.Create())
        {
            SrsCardId = cardId,
            SrsSessionItemId = sessionItemId,
            QuizAttemptItemId = quizAttemptItemId,
            Rating = rating,
            Quality = quality,
            ReviewedAt = reviewedAt,
            ElapsedMs = elapsedMs,
            PreviousDueAt = previousDueAt,
            NextDueAt = nextDueAt,
            PreviousIntervalDays = previousIntervalDays,
            NextIntervalDays = nextIntervalDays,
            PreviousEasinessFactor = previousEasinessFactor,
            NextEasinessFactor = nextEasinessFactor,
            PreviousRepetitions = previousRepetitions,
            NextRepetitions = nextRepetitions,
            CreatedAt = createdAt
        };
    }
}
