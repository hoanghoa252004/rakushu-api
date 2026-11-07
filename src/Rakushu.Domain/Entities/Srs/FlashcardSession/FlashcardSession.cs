using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.FlashcardSession.FlashcardSessionItem;

namespace Rakushu.Domain.Entities.Srs.FlashcardSession;

public sealed class FlashcardSession : AggregateRoot<FlashcardSessionId>
{
	public static readonly TimeSpan DefaultIdleTimeout = TimeSpan.FromMinutes(15);

	public UserId UserId { get; private set; } = null!;
	public FlashcardSessionStatus Status { get; private set; } = FlashcardSessionStatus.InProgress;
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset ExpiresAt { get; private set; }
	public DateTimeOffset? CompletedAt { get; private set; }

	private readonly List<FlashcardSessionItem.FlashcardSessionItem> _items = [];
	public IReadOnlyCollection<FlashcardSessionItem.FlashcardSessionItem> Items => _items.AsReadOnly();

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

	public void AddItem(FlashcardSessionItem.FlashcardSessionItem item)
	{
		_items.Add(item);
	}

	public bool HasAnyReviewedItems => _items.Any(i => i.Rating != null);

	public IReadOnlyList<FlashcardSessionItem.FlashcardSessionItem> PruneUnratedItems()
	{
		var unrated = _items.Where(i => i.Rating == null).ToList();
		_items.RemoveAll(i => i.Rating == null);
		return unrated;
	}

	public IReadOnlyList<FlashcardSessionItem.FlashcardSessionItem> ExpireAndPrune(DateTimeOffset now)
	{
		Expire(now);
		return PruneUnratedItems();
	}

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
}
