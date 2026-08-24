namespace Rakushu.Application.Usecases.Auth.Login;

public record LoginResponseDto(
	Guid UserId,
	string Username,
	string Email,
	string Role,
	string DisplayName,
	string? AvatarUrl,
	string AccessToken,
	string RefreshToken,
	DateTimeOffset AccessTokenExpiresAt
);
