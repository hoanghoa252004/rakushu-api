using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingRoleLine;

public static class ShadowingRoleLineErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"SHADOWING_ROLE_LINE.NOT_FOUND",
		"The shadowing role line was not found.");

	public static readonly Error InvalidSortOrder = Error.Validation(
		"SHADOWING_ROLE_LINE.INVALID_SORT_ORDER",
		"Shadowing role line sort order must be greater than 0.");

	public static readonly Error InvalidText = Error.Validation(
		"SHADOWING_ROLE_LINE.INVALID_TEXT",
		"Shadowing role line text is required.");

	public static readonly Error InvalidTimeRange = Error.Validation(
		"SHADOWING_ROLE_LINE.INVALID_TIME_RANGE",
		"Shadowing role line end time must be greater than start time and start time cannot be negative.");

	public static readonly Error InvalidAudioKey = Error.Validation(
		"SHADOWING_ROLE_LINE.INVALID_AUDIO_KEY",
		"Shadowing role line audio key is required and cannot exceed 500 characters.");

	public static readonly Error SortOrderAlreadyExists = Error.Conflict(
		"SHADOWING_ROLE_LINE.SORT_ORDER_ALREADY_EXISTS",
		"A role line with this sort order already exists in the shadowing material.");

	public static readonly Error RoleNotInMaterial = Error.Validation(
		"SHADOWING_ROLE_LINE.ROLE_NOT_IN_MATERIAL",
		"The role does not belong to the shadowing material.");
}
