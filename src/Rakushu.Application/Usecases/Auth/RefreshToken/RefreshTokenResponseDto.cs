namespace Rakushu.Application.Usecases.Auth.RefreshToken;

public sealed record RefreshTokenResponseDto(
	string AccessToken,
	string RefreshToken,
	DateTimeOffset AccessTokenExpiresAt
);
