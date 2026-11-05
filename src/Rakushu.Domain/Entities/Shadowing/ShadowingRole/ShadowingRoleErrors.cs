using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Shadowing.ShadowingRole;

public static class ShadowingRoleErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"SHADOWING_ROLE.NOT_FOUND",
		"The shadowing role was not found.");

	public static readonly Error InvalidSortOrder = Error.Validation(
		"SHADOWING_ROLE.INVALID_SORT_ORDER",
		"Shadowing role sort order must be greater than 0.");

	public static readonly Error SpeakerAlreadyExists = Error.Conflict(
		"SHADOWING_ROLE.SPEAKER_ALREADY_EXISTS",
		"A role for this speaker already exists in the shadowing material.");

	public static readonly Error SortOrderAlreadyExists = Error.Conflict(
		"SHADOWING_ROLE.SORT_ORDER_ALREADY_EXISTS",
		"A role with this sort order already exists in the shadowing material.");
}
