using Rakushu.Domain.Common;

namespace Rakushu.Domain.Entities.Srs;

public sealed class SrsSessionItem : Entity<SrsSessionItemId>
{
    public SrsSessionId SrsSessionId { get; private set; } = null!;
    public SrsCardId SrsCardId { get; private set; } = null!;
    public int Position { get; private set; }
    public string? Rating { get; private set; }
    public DateTimeOffset ShownAt { get; private set; }
    public DateTimeOffset? ReviewedAt { get; private set; }
    public int? ElapsedMs { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public SrsSession SrsSession { get; private set; } = null!;
    public SrsCard SrsCard { get; private set; } = null!;

    private SrsSessionItem() { }

    private SrsSessionItem(SrsSessionItemId id, SrsSessionId sessionId, SrsCardId cardId, int position, DateTimeOffset shownAt)
        : base(id)
    {
        SrsSessionId = sessionId;
        SrsCardId = cardId;
        Position = position;
        ShownAt = shownAt;
        CreatedAt = shownAt;
    }

    internal static SrsSessionItem Create(SrsSessionId sessionId, SrsCardId cardId, int position, DateTimeOffset shownAt) =>
        new(SrsSessionItemId.Create(), sessionId, cardId, position, shownAt);

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
