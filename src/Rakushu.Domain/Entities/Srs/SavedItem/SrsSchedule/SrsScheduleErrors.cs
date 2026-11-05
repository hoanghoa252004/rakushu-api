using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Srs.SavedItem.SrsSchedule;

public static class SrsScheduleErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"SRS_SCHEDULE.NOT_FOUND", "The SRS schedule was not found.");

	public static readonly Error InvalidSavedItemId = Error.Validation(
		"SRS_SCHEDULE.INVALID_SAVED_ITEM_ID", "Saved item ID cannot be empty.");

	public static readonly Error AlreadySuspended = Error.Conflict(
		"SRS_SCHEDULE.ALREADY_SUSPENDED", "The vocabulary item is already suspended.");

	public static readonly Error NotSuspended = Error.Conflict(
		"SRS_SCHEDULE.NOT_SUSPENDED", "The vocabulary item is not currently suspended.");

	public static readonly Error AlreadyIgnored = Error.Conflict(
		"SRS_SCHEDULE.ALREADY_IGNORED", "The vocabulary item is already ignored.");
}
