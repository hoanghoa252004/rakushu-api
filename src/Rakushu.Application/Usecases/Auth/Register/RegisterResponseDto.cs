namespace Rakushu.Application.Usecases.Auth.Register;

public sealed record RegisterResponseDto(
	Guid UserId,
	string Username,
	string Email,
	string Role,
	string DisplayName,
	string AccessToken,
	string RefreshToken,
	DateTimeOffset AccessTokenExpiresAt
);
