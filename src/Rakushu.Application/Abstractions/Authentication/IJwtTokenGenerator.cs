namespace Rakushu.Application.Abstractions.Authentication;

public interface IJwtTokenGenerator
{
	string GenerateAccessToken(Guid userId, string email, string username, string roleName);
	(string Token, DateTimeOffset ExpiresAt) GenerateRefreshToken();
	int GetRefreshTokenExpirationDays();
	int GetAccessTokenExpirationMinutes();
}
