using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel;

public static class ProficiencyLevelErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"PROFICIENCY_LEVEL.NOT_FOUND", "The proficiency level was not found.");

	public static readonly Error InvalidName = Error.Validation(
		"PROFICIENCY_LEVEL.INVALID_NAME", "Level name is required and cannot exceed 200 characters.");

	public static readonly Error InvalidCode = Error.Validation(
		"PROFICIENCY_LEVEL.INVALID_CODE", "Level code is required and cannot exceed 50 characters.");
}
