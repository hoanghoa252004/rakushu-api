using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Domain.Entities.Srs.FlashcardSession.FlashcardSessionItem;

namespace Rakushu.Domain.Entities.Srs.FlashcardSession;

public sealed class FlashcardSession : AggregateRoot<FlashcardSessionId>
{
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
		TimeSpan? timeout = null)
	{
		if (userId is null || userId.Value == Guid.Empty)
			return Result.Failure<FlashcardSession>(FlashcardSessionErrors.InvalidUserId);

		var effectiveTimeout = timeout ?? TimeSpan.FromHours(1);

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

	public bool IsExpired(DateTimeOffset now) =>
		Status == FlashcardSessionStatus.InProgress && now > ExpiresAt;

	public Result Submit(DateTimeOffset now)
	{
		if (Status != FlashcardSessionStatus.InProgress)
			return Result.Failure(FlashcardSessionErrors.NotInProgress);

		if (now > ExpiresAt)
		{
			Status = FlashcardSessionStatus.Abandoned;
			return Result.Failure(FlashcardSessionErrors.Expired);
		}

		Status = FlashcardSessionStatus.Completed;
		CompletedAt = now;
		return Result.Success();
	}

	public Result Cancel(DateTimeOffset now)
	{
		if (Status != FlashcardSessionStatus.InProgress)
			return Result.Failure(FlashcardSessionErrors.NotInProgress);

		if (now > ExpiresAt)
		{
			Status = FlashcardSessionStatus.Abandoned;
			return Result.Failure(FlashcardSessionErrors.Expired);
		}

		Status = FlashcardSessionStatus.Abandoned;
		return Result.Success();
	}
}
