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

	public static readonly Error ParentNotFound = Error.Validation(
		"CONTENT_CATEGORY.PARENT_NOT_FOUND", "Parent category not found.");

	public static readonly Error DuplicateDisplayOrder = Error.Validation(
		"CONTENT_CATEGORY.DUPLICATE_DISPLAY_ORDER", "Display order must be unique within each level.");

	public static readonly Error IsInUse = Error.Conflict(
		"CONTENT_CATEGORY.IN_USE", "Cannot delete category that is currently in use (has related content).");

	public static readonly Error InvalidThemeColor = Error.Validation(
		"CONTENT_CATEGORY.INVALID_THEME_COLOR", "Theme color is required and must be a valid hex color code.");

	public static readonly Error InvalidDisplayOrder = Error.Validation(
		"CONTENT_CATEGORY.INVALID_DISPLAY_ORDER", "Display order must be a positive integer. 1,2,3...)");

	public static readonly Error DuplicateCode = Error.Conflict(
		"CONTENT_CATEGORY.DUPLICATE_CODE", "A content category with this code already exists.");
}
