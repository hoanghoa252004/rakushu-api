using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Domain.Entities.User;
using Rakushu.Domain.Entities.User.RefreshToken;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Rakushu.Infrastructure.Authentication;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
	private readonly JwtSettings _jwtSettings;

	public JwtTokenGenerator(IOptions<JwtSettings> jwtOptions)
	{
		_jwtSettings = jwtOptions.Value;
	}

	public string GenerateAccessToken(UserId userId, string role, DateTimeOffset expiredDate)
	{
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var claims = new List<Claim>
		{
			new(JwtRegisteredClaimNames.Sub, userId.ToString()),
			new(ClaimTypes.Role, role),
			new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		};

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = expiredDate.UtcDateTime,
			Issuer = _jwtSettings.Issuer,
			Audience = _jwtSettings.Audience,
			SigningCredentials = credentials
		};

		var tokenHandler = new JwtSecurityTokenHandler();

		var token = tokenHandler.CreateToken(tokenDescriptor);

		return tokenHandler.WriteToken(token);
	}

	public string GenerateHashedToken()
	{
		var randomNumber = new byte[64];

		using var rng = RandomNumberGenerator.Create();

		rng.GetBytes(randomNumber);

		var hashedToken = Convert.ToBase64String(randomNumber);

		return hashedToken;
	}

	public int GetAccessTokenExpirationMinutes() => _jwtSettings.AccessTokenExpirationMinutes;

	public int GetRefreshTokenExpirationDays() => _jwtSettings.RefreshTokenExpirationDays;
}
