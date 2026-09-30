using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.ProficiencyFramework.ProficiencyLevel.ProficiencyEquivalence;

public static class ProficiencyEquivalenceErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"PROFICIENCY_EQUIVALENCE.NOT_FOUND", "The proficiency equivalence was not found.");
}
