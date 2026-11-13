using Rakushu.Domain.Common.Errors;
using Rakushu.Domain.Common.Results;

namespace Rakushu.Application.Common.Parsing;

public static class EnumParser
{
	// Case-insensitive name match only; numeric values are rejected.
	public static Result<TEnum> Parse<TEnum>(string? value, string fieldName) where TEnum : struct, Enum
	{
		if (string.IsNullOrWhiteSpace(value)
			|| !Enum.TryParse<TEnum>(value.Trim(), true, out var parsed)
			|| !Enum.IsDefined(parsed)
			|| char.IsDigit(value.Trim()[0]) || value.Trim()[0] == '-')
		{
			return Result.Failure<TEnum>(Error.Validation(
				"COMMON.INVALID_ENUM",
				$"'{value}' is not a valid {fieldName}. Allowed values: {string.Join(", ", Enum.GetNames<TEnum>())}."));
		}

		return Result.Success(parsed);
	}

	// Null/empty yields success with null (optional filter).
	public static Result<TEnum?> ParseOptional<TEnum>(string? value, string fieldName) where TEnum : struct, Enum
	{
		if (string.IsNullOrWhiteSpace(value))
			return Result.Success<TEnum?>(null);

		var parsed = Parse<TEnum>(value, fieldName);
		return parsed.IsFailure
			? Result.Failure<TEnum?>(parsed.Error)
			: Result.Success<TEnum?>(parsed.Value);
	}
}
