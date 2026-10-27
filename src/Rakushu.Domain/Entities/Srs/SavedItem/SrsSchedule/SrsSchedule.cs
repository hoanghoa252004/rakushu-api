using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsItemDetail;

namespace Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

public sealed class SrsSchedule : Entity<SrsScheduleId>
{
	public SavedItemId SavedItemId { get; private set; } = null!;
	public SrsStatus Status { get; private set; } = SrsStatus.New;
	public DateTimeOffset DueAt { get; private set; }
	public DateTimeOffset? LastReviewedAt { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	public SavedItem SavedItem { get; private set; } = null!;

	private readonly List<SrsItemDetail.SrsItemDetail> _details = [];
	public IReadOnlyCollection<SrsItemDetail.SrsItemDetail> Details => _details.AsReadOnly();

	private SrsSchedule() { }
	private SrsSchedule(SrsScheduleId id) : base(id) { }

	public static Result<SrsSchedule> Create(
		SavedItemId savedItemId,
		DateTimeOffset now)
	{
		if (savedItemId is null || savedItemId.Value == Guid.Empty)
			return Result.Failure<SrsSchedule>(SrsScheduleErrors.InvalidSavedItemId);

		return Result.Success(new SrsSchedule(SrsScheduleId.Create())
		{
			SavedItemId = savedItemId,
			Status = SrsStatus.New,
			DueAt = now,
			CreatedAt = now,
			UpdatedAt = now
		});
	}

	public void UpdateSchedule(SrsStatus status, DateTimeOffset dueAt, DateTimeOffset reviewedAt)
	{
		Status = status;
		DueAt = dueAt;
		LastReviewedAt = reviewedAt;
		UpdatedAt = reviewedAt;
	}

	public Result Suspend(DateTimeOffset now)
	{
		if (Status == SrsStatus.Suspended)
			return Result.Failure(SrsScheduleErrors.AlreadySuspended);

		Status = SrsStatus.Suspended;
		UpdatedAt = now;
		return Result.Success();
	}

	public Result Resume(SrsStatus targetStatus, DateTimeOffset now)
	{
		if (Status != SrsStatus.Suspended)
			return Result.Failure(SrsScheduleErrors.NotSuspended);

		Status = targetStatus;
		UpdatedAt = now;
		return Result.Success();
	}
}
