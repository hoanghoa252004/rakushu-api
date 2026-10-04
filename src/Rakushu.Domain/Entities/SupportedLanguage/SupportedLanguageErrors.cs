using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.SupportedLanguage;

public static class SupportedLanguageErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"SUPPORTED_LANGUAGE.NOT_FOUND", "The supported language was not found.");

	public static readonly Error InvalidCode = Error.Validation(
		"SUPPORTED_LANGUAGE.INVALID_CODE", "Language code is required and cannot exceed 10 characters.");

	public static readonly Error InvalidName = Error.Validation(
		"SUPPORTED_LANGUAGE.INVALID_NAME", "Language name is required and cannot exceed 100 characters.");

	public static readonly Error InvalidNativeName = Error.Validation(
		"SUPPORTED_LANGUAGE.INVALID_NATIVE_NAME", "Language native name is required and cannot exceed 100 characters.");

	public static readonly Error NotActive = Error.Validation(
		"SUPPORTED_LANGUAGE.NOT_ACTIVE", "Supported language is not active.");
}
