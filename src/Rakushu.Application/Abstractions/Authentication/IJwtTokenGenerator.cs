using Rakushu.Domain.Entities;

namespace Rakushu.Application.Abstractions.Authentication;

public interface IJwtTokenGenerator
{
	string GenerateAccessToken(User user, string roleName);
	string GenerateRefreshToken();
	int GetRefreshTokenExpirationDays();
	int GetAccessTokenExpirationMinutes();
}
