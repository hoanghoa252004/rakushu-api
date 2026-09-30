using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.User.Profile;

public static class ProfileErrors
{
	public static readonly Error InvalidFullName = Error.Validation(
		"USER.PROFILE.INVALID_FULLNAME", "FullName is required & cannot exceed 50 characters.");

	public static readonly Error NotFound = Error.NotFound(
		"USER.PROFILE.NOT_FOUND", "Profile was not found.");

	public static readonly Error AlreadyExists = Error.Conflict(
		"USER.PROFILE.ALREADY_EXISTS", "User already has a profile.");

	public static readonly Error InterestNotFound = Error.NotFound(
		"USER.PROFILE.INTEREST_NOT_FOUND", "Interest was not found.");
}
