using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Srs.SavedItem.SrsImmersionBoostLog;

public static class SrsImmersionBoostLogErrors
{
	public static readonly Error InvalidSrsScheduleId = Error.Validation(
		"SRS_IMMERSION_BOOST_LOG.INVALID_SCHEDULE_ID", "SRS schedule ID cannot be empty.");

	public static readonly Error InvalidAdjustmentType = Error.Validation(
		"SRS_IMMERSION_BOOST_LOG.INVALID_ADJUSTMENT_TYPE", "Adjustment type must be OverdueRescue or DueTodayBonus.");

	public static readonly Error InvalidBonusDays = Error.Validation(
		"SRS_IMMERSION_BOOST_LOG.INVALID_BONUS_DAYS", "Bonus days must be greater than zero.");

	public static readonly Error InvalidBoostNumber = Error.Validation(
		"SRS_IMMERSION_BOOST_LOG.INVALID_BOOST_NUMBER", "Boost number must be at least 1.");

	public static readonly Error InvalidEncounterCount = Error.Validation(
		"SRS_IMMERSION_BOOST_LOG.INVALID_ENCOUNTER_COUNT", "Encounter count in window must be at least 1.");
}
