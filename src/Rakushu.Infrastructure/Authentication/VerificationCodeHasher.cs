using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Application.Abstractions.Infrastructure.Configuration;
using Rakushu.Domain.Entities.Configuration.SystemConfiguration;
using System.Security.Cryptography;
using System.Text;

namespace Rakushu.Infrastructure.Authentication;

internal sealed class VerificationCodeHasher : IVerificationCodeHasher
{
	private readonly ISystemConfigurationProvider _configurationProvider;

	public VerificationCodeHasher(ISystemConfigurationProvider configurationProvider)
	{
		_configurationProvider = configurationProvider;
	}

	public string Generate6DigitCode()
	{
		return RandomNumberGenerator
			.GetInt32(100000, 1000000)
			.ToString();
	}

	public DateTimeOffset GetExpirationTime(DateTimeOffset now)
	{
		return now.AddMinutes(_configurationProvider.GetInt(SystemConfigurationKey.JwtSettings_EmailVerificationTokenExpirationMinutes));
	}

	public string Hash(string code)
	{
		var bytes = Encoding.UTF8.GetBytes(code);
		var hash = SHA256.HashData(bytes);

		return Convert.ToHexString(hash);
	}

	public bool Verify(string code, string expectedHash)
	{
		var actualHash = Hash(code);

		return CryptographicOperations.FixedTimeEquals(
			Convert.FromHexString(actualHash),
			Convert.FromHexString(expectedHash));
	}
}
