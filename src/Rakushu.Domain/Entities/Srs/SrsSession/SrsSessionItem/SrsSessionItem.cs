namespace Rakushu.Domain.Entities.Srs;

public sealed class SrsSessionItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid SrsSessionId { get; private set; }
    public Guid SrsCardId { get; private set; }
    public int Position { get; private set; }
    public string? Rating { get; private set; }
    public DateTimeOffset ShownAt { get; private set; }
    public DateTimeOffset? ReviewedAt { get; private set; }
    public int? ElapsedMs { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public SrsSession SrsSession { get; private set; } = null!;
    public SrsCard SrsCard { get; private set; } = null!;

    private SrsSessionItem() { }

    internal static SrsSessionItem Create(Guid sessionId, Guid cardId, int position, DateTimeOffset shownAt) => new()
    {
        SrsSessionId = sessionId,
        SrsCardId = cardId,
        Position = position,
        ShownAt = shownAt,
        CreatedAt = shownAt
    };

    internal Rakushu.Domain.Common.Results.Result RecordReview(string rating, DateTimeOffset reviewedAt, int? elapsedMs)
    {
        if (ReviewedAt is not null)
            return Rakushu.Domain.Common.Results.Result.Failure(SrsSessionErrors.SessionItemAlreadyReviewed);
        if (string.IsNullOrWhiteSpace(rating) || !SrsReviewRating.IsValid(rating))
            return Rakushu.Domain.Common.Results.Result.Failure(SrsSessionErrors.InvalidReviewRating);
        if (reviewedAt < ShownAt || (elapsedMs.HasValue && elapsedMs.Value < 0))
            return Rakushu.Domain.Common.Results.Result.Failure(SrsSessionErrors.InvalidReviewTiming);

        Rating = rating;
        ReviewedAt = reviewedAt;
        ElapsedMs = elapsedMs;
        return Rakushu.Domain.Common.Results.Result.Success();
    }
}