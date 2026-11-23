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

	public static readonly Error InvalidCharRange = Error.Validation(
		"ISSUE_DETAIL.INVALID_CHAR_RANGE", "Character range [startCharIndex, endCharIndex) must be non-negative and endCharIndex must be greater than startCharIndex.");

	public static readonly Error ShadowingAttemptNotAssessed = Error.Conflict(
		"ISSUE_DETAIL.SHADOWING_ATTEMPT_NOT_ASSESSED", "Only assessed shadowing attempts can be requested for curator re-evaluation.");

	public static readonly Error ShadowingAlreadyReevaluated = Error.Conflict(
		"ISSUE_DETAIL.SHADOWING_ALREADY_REEVALUATED", "This shadowing attempt has already been re-evaluated by a curator.");

	public static readonly Error ShadowingNoAssessedLines = Error.Validation(
		"ISSUE_DETAIL.SHADOWING_NO_ASSESSED_LINES", "Cannot request re-evaluation for a shadowing attempt with no non-skipped lines.");

	public static readonly Error ShadowingReevaluationRequestLimitReached = Error.Conflict(
		"ISSUE_DETAIL.SHADOWING_REEVALUATION_REQUEST_LIMIT_REACHED", "You have already submitted a curator re-evaluation request for this shadowing attempt.");
}
