using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Rakushu.Application.Abstractions.Infrastructure.Authentication;

public interface IVerificationCodeHasher
{
	string Generate6DigitCode();

	string Hash(string code);

	bool Verify(string code, string expectedHash);
	DateTimeOffset GetExpirationTime(DateTimeOffset now);
}
