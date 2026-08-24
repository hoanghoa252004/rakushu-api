using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Errors;

public static class DomainErrors
{
	public static class Auth
	{
		public static readonly Error InvalidCredentials =
			Error.Unauthorized("AUTH.INVALID_CREDENTIALS", "Invalid email or password.");

		public static readonly Error UserInactive =
			Error.Unauthorized("AUTH.USER_INACTIVE", "Your account is inactive or banned. Please contact admin.");

		public static readonly Error InvalidRefreshToken =
			Error.Unauthorized("AUTH.INVALID_REFRESH_TOKEN", "The refresh token is invalid or has expired.");

		public static readonly Error InvalidOrExpiredResetToken =
			Error.Validation("AUTH.INVALID_RESET_TOKEN", "The password reset token is invalid or has expired.");

		public static readonly Error PasswordMismatch =
			Error.Validation("AUTH.PASSWORD_MISMATCH", "Current password does not match.");

		public static readonly Error SamePassword =
			Error.Validation("AUTH.SAME_PASSWORD", "New password cannot be the same as the current password.");
	}

	public static class User
	{
		public static readonly Error NotFound =
			Error.NotFound("USER.NOT_FOUND", "User was not found.");

		public static readonly Error EmailAlreadyExists =
			Error.Conflict("USER.EMAIL_ALREADY_EXISTS", "A user with this email already exists.");

		public static readonly Error UsernameAlreadyExists =
			Error.Conflict("USER.USERNAME_ALREADY_EXISTS", "A user with this username already exists.");

		public static readonly Error RoleNotFound =
			Error.NotFound("USER.ROLE_NOT_FOUND", "Specified role was not found.");
	}

	public static class Profile
	{
		public static readonly Error NotFound =
			Error.NotFound("PROFILE.NOT_FOUND", "Profile was not found.");
	}
}
