using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using FlashcardSessionItemEntity = Rakushu.Domain.Entities.Srs.FlashcardSession.FlashcardSessionItem.FlashcardSessionItem;
using FlashcardSessionTargetEntity = Rakushu.Domain.Entities.Srs.FlashcardSession.FlashcardSessionTarget.FlashcardSessionTarget;

namespace Rakushu.Domain.Entities.Srs.FlashcardSession;

public sealed class FlashcardSession : AggregateRoot<FlashcardSessionId>
{
	public static readonly TimeSpan DefaultIdleTimeout = TimeSpan.FromMinutes(15);

	public UserId UserId { get; private set; } = null!;
	public FlashcardSessionStatus Status { get; private set; } = FlashcardSessionStatus.InProgress;
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset ExpiresAt { get; private set; }
	public DateTimeOffset? CompletedAt { get; private set; }

	private readonly List<FlashcardSessionTargetEntity> _targets = [];
	public IReadOnlyCollection<FlashcardSessionTargetEntity> Targets => _targets.AsReadOnly();

	private readonly List<FlashcardSessionItemEntity> _items = [];
	public IReadOnlyCollection<FlashcardSessionItemEntity> Items => _items.AsReadOnly();

	// Navigation Property
	public User User { get; private set; } = null!;

	private FlashcardSession() { }
	private FlashcardSession(FlashcardSessionId id) : base(id) { }

	public static Result<FlashcardSession> Create(
		UserId userId,
		DateTimeOffset now,
		TimeSpan? idleTimeout = null)
	{
		if (userId is null || userId.Value == Guid.Empty)
			return Result.Failure<FlashcardSession>(FlashcardSessionErrors.InvalidUserId);

		var effectiveTimeout = idleTimeout ?? DefaultIdleTimeout;

		return Result.Success(new FlashcardSession(FlashcardSessionId.Create())
		{
			UserId = userId,
			Status = FlashcardSessionStatus.InProgress,
			CreatedAt = now,
			ExpiresAt = now.Add(effectiveTimeout)
		});
	}

	public void AddTarget(FlashcardSessionTargetEntity target)
	{
		_targets.Add(target);
	}

	public void AddItem(FlashcardSessionItemEntity item)
	{
		_items.Add(item);
	}

	public bool HasAnyReviewedItems => _items.Count > 0;

	public bool IsExpired(DateTimeOffset now) =>
		Status == FlashcardSessionStatus.InProgress && now > ExpiresAt;

	public void Expire(DateTimeOffset now)
	{
		Status = FlashcardSessionStatus.Expired;
	}

	public void RefreshIdleTimeout(DateTimeOffset now, TimeSpan? idleTimeout = null)
	{
		if (Status != FlashcardSessionStatus.InProgress)
			return;

		var duration = idleTimeout ?? DefaultIdleTimeout;
		ExpiresAt = now.Add(duration);
	}

	public Result Complete(DateTimeOffset now)
	{
		if (Status == FlashcardSessionStatus.Completed)
			return Result.Failure(FlashcardSessionErrors.AlreadyCompleted);

		if (Status == FlashcardSessionStatus.Canceled)
			return Result.Failure(FlashcardSessionErrors.AlreadyCanceled);

		if (Status != FlashcardSessionStatus.InProgress)
			return Result.Failure(FlashcardSessionErrors.NotInProgress);

		if (now > ExpiresAt)
		{
			Status = FlashcardSessionStatus.Expired;
			return Result.Failure(FlashcardSessionErrors.Expired);
		}

		if (!HasAnyReviewedItems)
			return Result.Failure(FlashcardSessionErrors.NoItemsReviewed);

		Status = FlashcardSessionStatus.Completed;
		CompletedAt = now;
		return Result.Success();
	}

	public Result Cancel(DateTimeOffset now)
	{
		if (Status == FlashcardSessionStatus.Completed)
			return Result.Failure(FlashcardSessionErrors.AlreadyCompleted);

		if (Status == FlashcardSessionStatus.Canceled)
			return Result.Failure(FlashcardSessionErrors.AlreadyCanceled);

		if (Status != FlashcardSessionStatus.InProgress)
			return Result.Failure(FlashcardSessionErrors.NotInProgress);

		Status = FlashcardSessionStatus.Canceled;
		CompletedAt = now;
		return Result.Success();
	}
}
