namespace Rakushu.Application.Usecases.Authentication.Login;

public sealed record CredentialResponseDto(
	string AccessToken,
	DateTimeOffset AccessTokenExpiresAt,
	string RefreshToken,
	DateTimeOffset RefreshTokenExpiresAt
);
