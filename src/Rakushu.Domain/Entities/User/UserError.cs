using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.User;

public static class UserError
{
	public static readonly Error NotFound = Error.NotFound(
		"USER.NOT_FOUND", "The user with the specified identifier was not found.");

	public static readonly Error EmailAlreadyExists = Error.Conflict(
		"USER.EMAIL_ALREADY_EXISTS", "A user with this email address already exists.");

	public static readonly Error UsernameAlreadyExists = Error.Conflict(
		"USER.USERNAME_ALREADY_EXISTS", "A user with this username already exists.");

	public static readonly Error ProfileNotFound = Error.NotFound(
		"USER.PROFILE_NOT_FOUND", "Profile for the specified user was not found.");

	public static readonly Error InvalidCredentials = Error.Validation(
		"AUTH.INVALID_CREDENTIALS", "Invalid email or password.");

	public static readonly Error UserInactiveOrBannned = Error.Validation(
		"AUTH.USER_INACTIVE", "Your account is inactive or has been banned.");

	public static readonly Error UnverifiedYet = Error.Validation(
	"AUTH.USER_UNVERIFIED", "Your account has not verified email yet.");

	public static readonly Error InvalidRefreshToken = Error.Validation(
		"AUTH.INVALID_REFRESH_TOKEN", "The provided refresh token is invalid or expired.");

	public static readonly Error PasswordMismatch = Error.Validation(
		"AUTH.PASSWORD_MISMATCH", "Current password does not match.");

	public static readonly Error SamePassword = Error.Validation(
		"AUTH.SAME_PASSWORD", "New password cannot be the same as the current password.");

	public static readonly Error UnauthorizedResourceAccess = Error.Unauthorized(
		"AUTH.UNAUTHORIZED_RESOURCE_ACCESS", "You are not authorized to access this resource.");

	public static readonly Error NoNeedToVerify = Error.Failure(
		"AUTH.USER_NO_NEED_TO_VERIFY", "You do not need to verify your email anymore.");
}
