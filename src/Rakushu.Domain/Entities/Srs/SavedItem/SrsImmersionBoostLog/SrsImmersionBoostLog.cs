using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

namespace Rakushu.Domain.Entities.Srs.SavedItem.SrsImmersionBoostLog;

/// <summary>
/// Append-only audit log recording each passive immersion DueAt postponement triggered
/// by repeated natural video exposure (OverdueRescue or DueTodayBonus).
/// Keeps passive schedule shifts traceable alongside active recall reviews without mutating SM-2 IntervalDays.
/// </summary>
public sealed class SrsImmersionBoostLog : Entity<SrsImmersionBoostLogId>
{
	public SrsScheduleId SrsScheduleId { get; private set; } = null!;
	public string AdjustmentType { get; private set; } = null!;
	public int BonusDays { get; private set; }
	public DateTimeOffset PreviousDueAt { get; private set; }
	public DateTimeOffset NewDueAt { get; private set; }
	public int BoostNumber { get; private set; }
	public int EncounterCountInWindow { get; private set; }
	public DateTimeOffset BoostedAt { get; private set; }

	// NAVIGATION PROPERTY
	public SrsSchedule.SrsSchedule SrsSchedule { get; private set; } = null!;

	private SrsImmersionBoostLog() { }
	private SrsImmersionBoostLog(SrsImmersionBoostLogId id) : base(id) { }

	public static Result<SrsImmersionBoostLog> Create(
		SrsScheduleId srsScheduleId,
		string adjustmentType,
		int bonusDays,
		DateTimeOffset previousDueAt,
		DateTimeOffset newDueAt,
		int boostNumber,
		int encounterCountInWindow,
		DateTimeOffset boostedAt)
	{
		if (srsScheduleId is null || srsScheduleId.Value == Guid.Empty)
		{
			return Result.Failure<SrsImmersionBoostLog>(SrsImmersionBoostLogErrors.InvalidSrsScheduleId);
		}

		var normalizedAdjustmentType = adjustmentType?.Trim();
		if (!SrsImmersionAdjustmentTypes.IsValid(normalizedAdjustmentType))
		{
			return Result.Failure<SrsImmersionBoostLog>(SrsImmersionBoostLogErrors.InvalidAdjustmentType);
		}

		if (bonusDays <= 0)
		{
			return Result.Failure<SrsImmersionBoostLog>(SrsImmersionBoostLogErrors.InvalidBonusDays);
		}

		if (boostNumber < 1)
		{
			return Result.Failure<SrsImmersionBoostLog>(SrsImmersionBoostLogErrors.InvalidBoostNumber);
		}

		if (encounterCountInWindow < 1)
		{
			return Result.Failure<SrsImmersionBoostLog>(SrsImmersionBoostLogErrors.InvalidEncounterCount);
		}

		return Result.Success(new SrsImmersionBoostLog(SrsImmersionBoostLogId.Create())
		{
			SrsScheduleId = srsScheduleId,
			AdjustmentType = normalizedAdjustmentType!,
			BonusDays = bonusDays,
			PreviousDueAt = previousDueAt,
			NewDueAt = newDueAt,
			BoostNumber = boostNumber,
			EncounterCountInWindow = encounterCountInWindow,
			BoostedAt = boostedAt
		});
	}
}
