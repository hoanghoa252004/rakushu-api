using Rakushu.Domain.Entities.User;

namespace Rakushu.Application.Abstractions.Infrastructure.Authentication;

public interface IJwtTokenGenerator
{
	// ACCESS TOKEN
	string GenerateAccessToken(UserId userId, string role, DateTimeOffset expiredDate);
	int GetAccessTokenExpirationMinutes();

	// REFRESH TOKEN
	string GenerateHashedToken();
	int GetRefreshTokenExpirationDays();
	
}
