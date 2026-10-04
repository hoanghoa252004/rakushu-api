using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.LearningUnit.Bunsetsu.Token;

public static class TokenErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"TOKEN.NOT_FOUND", "The token was not found.");
}
