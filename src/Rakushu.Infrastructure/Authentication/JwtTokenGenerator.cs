using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Configuration;
using Rakushu.Domain.Entities.Configuration.SystemConfiguration;
using Rakushu.Domain.Entities.Identity.User;
using Rakushu.Infrastructure.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Rakushu.Infrastructure.Authentication;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
	private readonly JwtSettings _jwtSettings;
	private readonly ISystemConfigurationProvider _configurationProvider;

	public JwtTokenGenerator(IOptions<JwtSettings> jwtOptions, ISystemConfigurationProvider configurationProvider)
	{
		_jwtSettings = jwtOptions.Value;
		_configurationProvider = configurationProvider;
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

	public int GetAccessTokenExpirationMinutes()
		=> _configurationProvider.GetInt(SystemConfigurationKey.JwtSettings_AccessTokenExpirationMinutes);

	public int GetRefreshTokenExpirationDays()
		=> _configurationProvider.GetInt(SystemConfigurationKey.JwtSettings_RefreshTokenExpirationDays);
}
