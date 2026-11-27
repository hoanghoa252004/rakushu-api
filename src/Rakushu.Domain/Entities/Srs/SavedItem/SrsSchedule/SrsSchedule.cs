using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using SrsImmersionBoostLogEntity = Rakushu.Domain.Entities.Srs.SavedItem.SrsImmersionBoostLog.SrsImmersionBoostLog;
using SrsScheduleDetailEntity = Rakushu.Domain.Entities.Srs.SavedItem.SrsScheduleDetail.SrsScheduleDetail;

namespace Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

public sealed class SrsSchedule : Entity<SrsScheduleId>
{
	public const int ImmersionWindowDays = 7;
	public const int PassiveBoostEncounterThreshold = 3;
	public const int OverdueRescueDays = 2;
	public const int DueTodayBonusDays = 4;
	public const int MaxConsecutivePassiveBoosts = 2;

	public SavedItemId SavedItemId { get; private set; } = null!;
	public DateTimeOffset DueAt { get; private set; }
	public DateTimeOffset CreatedAt { get; private set; }
	public DateTimeOffset UpdatedAt { get; private set; }
	public DateTimeOffset? LastImmersionBoostAt { get; private set; }
	public int ConsecutivePassiveBoosts { get; private set; }

	// NAVIGATION PROPERTIES
	public SavedItem SavedItem { get; private set; } = null!;

	private readonly List<SrsScheduleDetailEntity> _details = [];
	public IReadOnlyCollection<SrsScheduleDetailEntity> Details => _details.AsReadOnly();

	private readonly List<SrsImmersionBoostLogEntity> _boostLogs = [];
	public IReadOnlyCollection<SrsImmersionBoostLogEntity> BoostLogs => _boostLogs.AsReadOnly();

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
			UpdatedAt = now,
			LastImmersionBoostAt = null,
			ConsecutivePassiveBoosts = 0
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

	/// <summary>
	/// Records an active recall review (Quiz or Flashcard), updating DueAt and resetting
	/// the consecutive passive immersion boost counter.
	/// </summary>
	public void RecordActiveReview(DateTimeOffset dueAt, DateTimeOffset now)
	{
		DueAt = dueAt;
		UpdatedAt = now;
		ConsecutivePassiveBoosts = 0;
	}

	/// <summary>
	/// Checks whether this schedule is eligible to receive a passive immersion DueAt adjustment today.
	/// Guards against exceeding max consecutive passive boosts, same-day double boosting, and shifting not-yet-due items.
	/// </summary>
	public bool CanReceivePassiveBoost(DateTimeOffset startOfToday, DateTimeOffset endOfToday)
	{
		if (ConsecutivePassiveBoosts >= MaxConsecutivePassiveBoosts)
		{
			return false;
		}

		if (LastImmersionBoostAt.HasValue && LastImmersionBoostAt.Value >= startOfToday)
		{
			return false;
		}

		return DueAt <= endOfToday;
	}

	/// <summary>
	/// Returns the exclusive watermark timestamp after which new effective encounters count toward
	/// the next passive immersion boost (the latest of LastImmersionBoostAt and the most recent active review).
	/// </summary>
	public DateTimeOffset? GetPassiveBoostExclusiveAfterUtc()
	{
		DateTimeOffset? latestReviewAt = _details.Count > 0
			? _details.OrderByDescending(d => d.ReviewNumber).First().ReviewedAt
			: null;

		if (LastImmersionBoostAt.HasValue && latestReviewAt.HasValue)
		{
			return LastImmersionBoostAt.Value >= latestReviewAt.Value
				? LastImmersionBoostAt.Value
				: latestReviewAt.Value;
		}

		return LastImmersionBoostAt ?? latestReviewAt;
	}

	/// <summary>
	/// Applies a passive immersion DueAt shift (OverdueRescue or DueTodayBonus),
	/// advancing the watermark, incrementing the consecutive passive boost counter,
	/// and appending an immutable audit log entry to BoostLogs.
	/// </summary>
	public Result<SrsImmersionBoostLogEntity> ApplyPassiveImmersionBoost(
		DateTimeOffset newDueAt,
		string adjustmentType,
		int bonusDays,
		int encounterCountInWindow,
		DateTimeOffset now)
	{
		if (ConsecutivePassiveBoosts >= MaxConsecutivePassiveBoosts)
		{
			return Result.Failure<SrsImmersionBoostLogEntity>(SrsScheduleErrors.PassiveBoostCapReached);
		}

		var previousDueAt = DueAt;
		var nextBoostNumber = ConsecutivePassiveBoosts + 1;

		var logResult = SrsImmersionBoostLogEntity.Create(
			Id,
			adjustmentType,
			bonusDays,
			previousDueAt,
			newDueAt,
			nextBoostNumber,
			encounterCountInWindow,
			now);

		if (logResult.IsFailure)
		{
			return logResult;
		}

		DueAt = newDueAt;
		UpdatedAt = now;
		LastImmersionBoostAt = now;
		ConsecutivePassiveBoosts = nextBoostNumber;
		_boostLogs.Add(logResult.Value);

		return logResult;
	}
}
