namespace Rakushu.Application.Usecases.Auth.Login;

public sealed record CredentialResponseDto(
	string AccessToken,
	DateTimeOffset AccessTokenExpiresAt,
	string RefreshToken,
	DateTimeOffset RefreshTokenExpiresAt
);
