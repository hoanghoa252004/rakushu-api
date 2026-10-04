using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.LearningUnit.Bunsetsu;

public static class BunsetsuErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"BUNSETSU.NOT_FOUND", "The bunsetsu was not found.");
}
