using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Rakushu.Application.Abstractions.Authentication;
using Rakushu.Domain.Entities;

namespace Rakushu.Infrastructure.Authentication;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
	private readonly JwtSettings _jwtSettings;

	public JwtTokenGenerator(IOptions<JwtSettings> jwtOptions)
	{
		_jwtSettings = jwtOptions.Value;
	}

	public string GenerateAccessToken(User user, string roleName)
	{
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var claims = new List<Claim>
		{
			new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
			new(JwtRegisteredClaimNames.Email, user.Email),
			new(JwtRegisteredClaimNames.UniqueName, user.Username),
			new(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new(ClaimTypes.Name, user.Username),
			new(ClaimTypes.Email, user.Email),
			new(ClaimTypes.Role, roleName),
			new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		};

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
			Issuer = _jwtSettings.Issuer,
			Audience = _jwtSettings.Audience,
			SigningCredentials = credentials
		};

		var tokenHandler = new JwtSecurityTokenHandler();
		var token = tokenHandler.CreateToken(tokenDescriptor);

		return tokenHandler.WriteToken(token);
	}

	public string GenerateRefreshToken()
	{
		var randomNumber = new byte[64];
		using var rng = RandomNumberGenerator.Create();
		rng.GetBytes(randomNumber);
		return Convert.ToBase64String(randomNumber);
	}

	public int GetAccessTokenExpirationMinutes() => _jwtSettings.AccessTokenExpirationMinutes;

	public int GetRefreshTokenExpirationDays() => _jwtSettings.RefreshTokenExpirationDays;
}
