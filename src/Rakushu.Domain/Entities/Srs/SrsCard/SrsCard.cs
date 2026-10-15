using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Domain.Entities.Srs;

public sealed class SrsCard : AggregateRoot<Guid>
{
    public Guid SrsNoteId { get; private set; }
    public SrsCardStatus Status { get; private set; } = SrsCardStatus.New;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public SrsNote SrsNote { get; private set; } = null!;
    public SrsCardState? State { get; private set; }

    private SrsCard() { }

    private SrsCard(Guid id, Guid srsNoteId, DateTimeOffset now) : base(id)
    {
        SrsNoteId = srsNoteId;
        CreatedAt = now;
        UpdatedAt = now;
        Status = SrsCardStatus.New;
    }

    public static Result<SrsCard> Create(Guid srsNoteId, DateTimeOffset now)
    {
        if (srsNoteId == Guid.Empty)
            return Result.Failure<SrsCard>(SrsCardErrors.InvalidNoteId);

        return Result.Success(new SrsCard(Guid.NewGuid(), srsNoteId, now));
    }

    public Result Suspend(DateTimeOffset now) => ChangeStatus(SrsCardStatus.Suspended, now, SrsCardStatus.New, SrsCardStatus.Learning, SrsCardStatus.Review);

    public Result Resume(DateTimeOffset now)
    {
        if (Status != SrsCardStatus.Suspended)
            return Result.Failure(SrsCardErrors.InvalidCardStatusTransition);

        Status = State is null || State.Repetitions == 0 ? SrsCardStatus.Learning : SrsCardStatus.Review;
        UpdatedAt = now;
        return Result.Success();
    }

    public Result Archive(DateTimeOffset now)
    {
        if (Status == SrsCardStatus.Archived)
            return Result.Failure(SrsCardErrors.CardAlreadyArchived);

        Status = SrsCardStatus.Archived;
        UpdatedAt = now;
        return Result.Success();
    }

    public Result AttachInitialState(SrsCardState state, DateTimeOffset now)
    {
        if (State is not null)
            return Result.Failure(SrsCardErrors.CardStateAlreadyExists);
        if (state.SrsCardId != Id)
            return Result.Failure(SrsCardErrors.CardStateOwnerMismatch);

        State = state;
        UpdatedAt = now;
        return Result.Success();
    }

    public Result RecordReview(string rating, DateTimeOffset reviewedAt)
    {
        if (Status is SrsCardStatus.Suspended or SrsCardStatus.Archived)
            return Result.Failure(SrsCardErrors.CardNotReviewable);
        if (State is null)
            return Result.Failure(SrsCardErrors.CardStateMissing);
        if (!SrsReviewRating.IsValid(rating))
            return Result.Failure(SrsCardErrors.InvalidReviewRating);

        var quality = rating.ToUpperInvariant() switch
        {
            "AGAIN" => 0,
            "HARD" => 3,
            "GOOD" => 4,
            "EASY" => 5,
            _ => throw new InvalidOperationException("Validated review rating was not mapped.")
        };

        State.ApplyReview(quality, reviewedAt);
        Status = State.Repetitions == 0 ? SrsCardStatus.Learning : SrsCardStatus.Review;
        UpdatedAt = reviewedAt;
        return Result.Success();
    }

    private Result ChangeStatus(SrsCardStatus nextStatus, DateTimeOffset now, params SrsCardStatus[] allowedCurrentStatuses)
    {
        if (!allowedCurrentStatuses.Contains(Status))
            return Result.Failure(SrsCardErrors.InvalidCardStatusTransition);

        Status = nextStatus;
        UpdatedAt = now;
        return Result.Success();
    }
}