using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Curation.Report;

public static class ReportErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"REPORT.NOT_FOUND", "The report was not found.");

	public static readonly Error InvalidTargetId = Error.Validation(
		"REPORT.INVALID_TARGET_ID", "Target ID cannot be empty.");

	public static readonly Error EmptyCategory = Error.Validation(
		"REPORT.EMPTY_CATEGORY", "Category name cannot be empty.");

	public static readonly Error InvalidStatusTransition = Error.Validation(
		"REPORT.INVALID_STATUS_TRANSITION", "Invalid status transition for report.");

	public static readonly Error AlreadyResolved = Error.Validation(
		"REPORT.ALREADY_RESOLVED", "The report has already been resolved.");

	public static readonly Error AlreadyDismissed = Error.Validation(
		"REPORT.ALREADY_DISMISSED", "The report has already been dismissed.");

	public static readonly Error EmptyCuratorNotes = Error.Validation(
		"REPORT.EMPTY_CURATOR_NOTES", "Curator notes cannot be empty.");
}
