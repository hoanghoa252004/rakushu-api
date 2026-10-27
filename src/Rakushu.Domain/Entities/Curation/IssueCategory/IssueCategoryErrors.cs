using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Curation.IssueCategory;

public static class IssueCategoryErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"ISSUE_CATEGORY.NOT_FOUND", "The issue category tracker was not found.");

	public static readonly Error InvalidTargetId = Error.Validation(
		"ISSUE_CATEGORY.INVALID_TARGET_ID", "Target ID cannot be empty.");

	public static readonly Error EmptyCategory = Error.Validation(
		"ISSUE_CATEGORY.EMPTY_CATEGORY", "Category name cannot be empty.");
}
