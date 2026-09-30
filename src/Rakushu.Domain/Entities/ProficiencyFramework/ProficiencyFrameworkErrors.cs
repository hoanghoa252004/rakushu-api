using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.ProficiencyFramework;

public static class ProficiencyFrameworkErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"PROFICIENCY_FRAMEWORK.NOT_FOUND", "The proficiency framework was not found.");

	public static readonly Error InvalidName = Error.Validation(
		"PROFICIENCY_FRAMEWORK.INVALID_NAME", "Framework name is required and cannot exceed 200 characters.");

	public static readonly Error InvalidCode = Error.Validation(
		"PROFICIENCY_FRAMEWORK.INVALID_CODE", "Framework code is required and cannot exceed 50 characters.");
}
