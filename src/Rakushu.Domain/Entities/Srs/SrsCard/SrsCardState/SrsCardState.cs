namespace Rakushu.Domain.Entities.Srs;

public sealed class SrsCardState
{
    public SrsCardId SrsCardId { get; private set; } = null!;
    public DateTimeOffset DueAt { get; private set; }
    public DateTimeOffset? LastReviewAt { get; private set; }
    public int Repetitions { get; private set; }
    public int Lapses { get; private set; }
    public double EasinessFactor { get; private set; } = 2.5;
    public int IntervalDays { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public SrsCard SrsCard { get; private set; } = null!;

    private SrsCardState() { }

    public static SrsCardState Create(SrsCardId cardId, DateTimeOffset now) => new()
    {
        SrsCardId = cardId,
        DueAt = now,
        UpdatedAt = now,
        EasinessFactor = 2.5,
        Repetitions = 0,
        Lapses = 0,
        IntervalDays = 0
    };

    public void ApplyReview(int quality, DateTimeOffset reviewedAt)
    {
        if (quality is < 0 or > 5)
            throw new ArgumentOutOfRangeException(nameof(quality), "SM-2 quality must be between 0 and 5.");
        if (LastReviewAt is not null && reviewedAt < LastReviewAt)
            throw new ArgumentOutOfRangeException(nameof(reviewedAt), "Review time cannot precede the previous review.");

        if (quality < 3)
        {
            Repetitions = 0;
            Lapses++;
            IntervalDays = 1;
        }
        else
        {
            IntervalDays = Repetitions switch
            {
                0 => 1,
                1 => 6,
                _ => Math.Max(1, (int)Math.Round(Math.Max(1, IntervalDays) * EasinessFactor, MidpointRounding.AwayFromZero))
            };
            Repetitions++;
        }

        EasinessFactor = Math.Max(1.3, EasinessFactor + (0.1 - (5 - quality) * (0.08 + (5 - quality) * 0.02)));
        LastReviewAt = reviewedAt;
        DueAt = reviewedAt.AddDays(IntervalDays);
        UpdatedAt = reviewedAt;
    }
}