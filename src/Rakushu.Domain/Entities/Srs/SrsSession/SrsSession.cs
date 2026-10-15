using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;

namespace Rakushu.Domain.Entities.Srs;

public sealed class SrsSession : AggregateRoot<Guid>
{
    public UserId UserId { get; private set; } = null!;
    public SrsSessionStatus Status { get; private set; } = SrsSessionStatus.InProgress;
    public int TotalItems { get; private set; }
    public int ReviewedItems { get; private set; }
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public User User { get; private set; } = null!;
    public ICollection<SrsSessionItem> Items { get; private set; } = new List<SrsSessionItem>();

    private SrsSession() { }
    private SrsSession(Guid id) : base(id) { }

    public static Result<SrsSession> Create(UserId userId, DateTimeOffset now)
    {
        if (userId is null)
            return Result.Failure<SrsSession>(SrsSessionErrors.InvalidUserId);

        return Result.Success(new SrsSession(Guid.NewGuid())
        {
            UserId = userId,
            Status = SrsSessionStatus.InProgress,
            StartedAt = now,
            CreatedAt = now,
            UpdatedAt = now
        });
    }

    public Result AddItem(SrsCard card, DateTimeOffset shownAt)
    {
        if (Status != SrsSessionStatus.InProgress)
            return Result.Failure(SrsSessionErrors.SessionNotInProgress);
        if (card is null || card.Id == Guid.Empty)
            return Result.Failure(SrsSessionErrors.InvalidCardId);
        if (Items.Any(x => x.SrsCardId == card.Id))
            return Result.Failure(SrsSessionErrors.DuplicateSessionCard);

        Items.Add(SrsSessionItem.Create(Id, card.Id, Items.Count + 1, shownAt));
        TotalItems = Items.Count;
        UpdatedAt = shownAt;
        return Result.Success();
    }

    public Result RecordReview(Guid itemId, string rating, DateTimeOffset reviewedAt, int? elapsedMs = null)
    {
        if (Status != SrsSessionStatus.InProgress)
            return Result.Failure(SrsSessionErrors.SessionNotInProgress);

        var item = Items.SingleOrDefault(x => x.Id == itemId);
        if (item is null)
            return Result.Failure(SrsSessionErrors.SessionItemNotFound);

        var result = item.RecordReview(rating, reviewedAt, elapsedMs);
        if (result.IsFailure)
            return result;

        ReviewedItems = Items.Count(x => x.ReviewedAt is not null);
        UpdatedAt = reviewedAt;
        return Result.Success();
    }

    public Result Complete(DateTimeOffset completedAt)
    {
        if (Status != SrsSessionStatus.InProgress)
            return Result.Failure(SrsSessionErrors.SessionNotInProgress);
        if (ReviewedItems != TotalItems)
            return Result.Failure(SrsSessionErrors.SessionHasUnreviewedItems);

        Status = SrsSessionStatus.Completed;
        CompletedAt = completedAt;
        UpdatedAt = completedAt;
        return Result.Success();
    }

    public Result Abandon(DateTimeOffset abandonedAt)
    {
        if (Status != SrsSessionStatus.InProgress)
            return Result.Failure(SrsSessionErrors.SessionNotInProgress);

        Status = SrsSessionStatus.Abandoned;
        UpdatedAt = abandonedAt;
        return Result.Success();
    }
}