using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Srs.SavedItem;

public static class SavedItemErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"SAVED_ITEM.NOT_FOUND", "The saved item was not found.");

	public static readonly Error InvalidUserId = Error.Validation(
		"SAVED_ITEM.INVALID_USER_ID", "User ID cannot be empty.");

	public static readonly Error InvalidMeaningId = Error.Validation(
		"SAVED_ITEM.INVALID_MEANING_ID", "Knowledge meaning ID cannot be empty.");

	public static readonly Error EmptyExpression = Error.Validation(
		"SAVED_ITEM.EMPTY_EXPRESSION", "Expression cannot be empty.");

	public static readonly Error EmptyMeaning = Error.Validation(
		"SAVED_ITEM.EMPTY_MEANING", "Meaning cannot be empty.");
}
