namespace Rakushu.Application.Usecases.Auth.Login;

public record LoginResponseDto(
	string AccessToken,
	string RefreshToken,
	DateTimeOffset AccessTokenExpiresAt
);
