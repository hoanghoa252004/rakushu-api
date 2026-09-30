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
}
