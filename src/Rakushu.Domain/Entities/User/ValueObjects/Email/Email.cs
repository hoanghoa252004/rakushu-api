using Rakushu.Domain.Common;
using Rakushu.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Rakushu.Domain.Entities.User.ValueObjects.Email;

public sealed partial class Email : ValueObject
{
	// MAIN PROPERTIES
	public string Value { get; private set; } = null!;

	// METADATA
	private const int MaxLength = 256;
	[GeneratedRegex(
		@"^[^@\s]+@[^@\s]+\.[^@\s]+$",
		RegexOptions.CultureInvariant)]
	private static partial Regex EmailRegex();

	// CONSTRUCTOR & FACTORY METHOD
	private Email(string value)
	{
		Value = value;
	}

	public static Result<Email> Create(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return Result.Failure<Email>(
				EmailErrors.Empty);

		if (value.Length > MaxLength)
			return Result.Failure<Email>(
				EmailErrors.TooLong);

		if (!EmailRegex().IsMatch(value))
			return Result.Failure<Email>(
				EmailErrors.InvalidFormat);

		return Result.Success(NormalizeEmail(value));
	}

	public static Email NormalizeEmail(string value)
	{
		return new Email(value.Trim().ToLower());
	}

	protected override IEnumerable<object?> GetEqualityComponents()
	{
		yield return Value;
	}

	public override string ToString()
	{
		return Value;
	}
}
