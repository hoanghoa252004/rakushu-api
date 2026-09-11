using Microsoft.Extensions.Options;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;
using Rakushu.Infrastructure.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Infrastructure.Authentication;

internal sealed class VerificationCodeHasher : IVerificationCodeHasher
{
	private readonly JwtSettings _jwtSettings;

	public VerificationCodeHasher(IOptions<JwtSettings> jwtOptions)
	{
		_jwtSettings = jwtOptions.Value;
	}

	public string Generate6DigitCode()
	{
		return RandomNumberGenerator
			.GetInt32(100000, 1000000)
			.ToString();
	}

	public DateTimeOffset GetExpirationTime(DateTimeOffset now)
	{
		return now.AddMinutes(_jwtSettings.EmailVerificationTokenExipationMinutes);
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