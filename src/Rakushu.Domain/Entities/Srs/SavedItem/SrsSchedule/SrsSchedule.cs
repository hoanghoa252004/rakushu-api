using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using SrsScheduleDetailEntity = Rakushu.Domain.Entities.Srs.SavedItem.SrsScheduleDetail.SrsScheduleDetail;

namespace Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

public sealed class SrsSchedule : Entity<SrsScheduleId>
{
	public SavedItemId SavedItemId { get; private set; } = null!;
	public DateTimeOffset DueAt { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }

	// NAVIGATION PROPERTIES
	public SavedItem SavedItem { get; private set; } = null!;

	private readonly List<SrsScheduleDetailEntity> _details = [];
	public IReadOnlyCollection<SrsScheduleDetailEntity> Details => _details.AsReadOnly();

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
			DueAt = now,
			CreatedAt = now,
			UpdatedAt = now
		});
	}

	public void AddDetail(SrsScheduleDetailEntity detail)
	{
		_details.Add(detail);
	}

	public void UpdateSchedule(DateTimeOffset dueAt, DateTimeOffset now)
	{
		DueAt = dueAt;
		UpdatedAt = now;
	}
}
