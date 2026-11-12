using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Curation.Report.IssueDetail;

public static class IssueDetailErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"ISSUE_DETAIL.NOT_FOUND", "The issue detail was not found.");

	public static readonly Error InvalidUserId = Error.Validation(
		"ISSUE_DETAIL.INVALID_USER_ID", "User ID cannot be empty.");

	public static readonly Error InvalidTargetId = Error.Validation(
		"ISSUE_DETAIL.INVALID_TARGET_ID", "Target ID cannot be empty.");

	public static readonly Error EmptyCategory = Error.Validation(
		"ISSUE_DETAIL.EMPTY_CATEGORY", "Category name cannot be empty.");

	public static readonly Error AlreadyReported = Error.Conflict(
		"ISSUE_DETAIL.ALREADY_REPORTED", "You have already reported an issue for this item that is currently pending review.");

	public static readonly Error CannotDeleteUnderReview = Error.Validation(
		"ISSUE_DETAIL.CANNOT_DELETE_UNDER_REVIEW", "Cannot delete an issue report that is already under review or has been resolved.");
}
