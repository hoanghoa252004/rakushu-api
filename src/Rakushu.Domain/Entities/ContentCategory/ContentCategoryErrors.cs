using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.ContentCategory;

public static class ContentCategoryErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"CONTENT_CATEGORY.NOT_FOUND", "The content category was not found.");

	public static readonly Error InvalidName = Error.Validation(
		"CONTENT_CATEGORY.INVALID_NAME", "Category name is required and cannot exceed 100 characters.");

	public static readonly Error InvalidCode = Error.Validation(
		"CONTENT_CATEGORY.INVALID_CODE", "Category code is required and cannot exceed 50 characters.");

	public static readonly Error NotActive = Error.Validation(
		"CONTENT_CATEGORY.NOT_ACTIVE", "Category is not active.");

	public static readonly Error InvalidLevel = Error.Validation(
		"CONTENT_CATEGORY.INVALID_LEVEL", "Category level must be a positive integer.");

	public static readonly Error ParentNotFound = Error.Validation(
		"CONTENT_CATEGORY.PARENT_NOT_FOUND", "Parent category not found.");

	public static readonly Error InvalidParentLevel = Error.Validation(
		"CONTENT_CATEGORY.INVALID_PARENT_LEVEL", "Parent level must be less than current level.");

	public static readonly Error DuplicateDisplayOrder = Error.Validation(
		"CONTENT_CATEGORY.DUPLICATE_DISPLAY_ORDER", "Display order must be unique within each level.");

	public static readonly Error IsInUse = Error.Conflict(
		"CONTENT_CATEGORY.IN_USE", "Cannot delete category that is currently in use (has related content).");

	public static readonly Error InvalidStatusTransition = Error.Failure(
		"CONTENT_CATEGORY.INVALID_STATUS_TRANSITION", "Invalid status transition. " +
		"From Draft: can go to Active or Inactive. " +
		"From Active: can go to Inactive. " +
		"From Inactive: can go to Active or Archived. " +
		"From Archived: no transitions allowed.");
}
