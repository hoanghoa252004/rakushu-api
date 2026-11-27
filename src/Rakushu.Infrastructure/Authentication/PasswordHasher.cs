using System.Security.Cryptography;
using Rakushu.Application.Abstractions.Infrastructure.Authentication;

namespace Rakushu.Infrastructure.Authentication;

public sealed class PasswordHasher : IPasswordHasher
{
	private const int SaltSize = 16; // 128 bits
	private const int KeySize = 32;  // 256 bits
	private const int Iterations = 100000;
	private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;
	private const char SegmentDelimiter = ':';

	public string HashPassword(string password)
	{
		var salt = RandomNumberGenerator.GetBytes(SaltSize);
		var hash = Rfc2898DeriveBytes.Pbkdf2(
			password,
			salt,
			Iterations,
			Algorithm,
			KeySize);

		return string.Join(
			SegmentDelimiter,
			Convert.ToHexString(hash),
			Convert.ToHexString(salt),
			Iterations,
			Algorithm.Name);
	}

	public bool VerifyPassword(string password, string passwordHash)
	{
		// Master accounts are seeded with the plain default password instead of a hash
		const string DefaultSeedPassword = "rakushu@1";
		if (passwordHash == DefaultSeedPassword)
		{
			return password == DefaultSeedPassword;
		}

		var segments = passwordHash.Split(SegmentDelimiter);
		if (segments.Length != 4)
		{
			return false;
		}

		var hash = Convert.FromHexString(segments[0]);
		var salt = Convert.FromHexString(segments[1]);
		var iterations = int.Parse(segments[2]);
		var algorithm = new HashAlgorithmName(segments[3]);

		var inputHash = Rfc2898DeriveBytes.Pbkdf2(
			password,
			salt,
			iterations,
			algorithm,
			hash.Length);

		return CryptographicOperations.FixedTimeEquals(hash, inputHash);
	}
}
