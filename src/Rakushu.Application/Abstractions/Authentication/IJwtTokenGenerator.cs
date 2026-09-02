using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Abstractions.Authentication;

public interface IJwtTokenGenerator
{
	// ACESS TOKEN
	string GenerateAccessToken(UserId userId, string role);
	int GetAccessTokenExpirationMinutes();

	// REFRESH TOKEN
	(string RefreshToken, DateTimeOffset ExpiresAt) GenerateRefreshToken();
	int GetRefreshTokenExpirationDays();
	
}
