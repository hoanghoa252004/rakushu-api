using Rakushu.Domain.Common.Errors;

namespace Rakushu.Domain.Entities.Linguistic;

public static class LinguisticErrors
{
	public static readonly Error NotFound = Error.NotFound(
		"LINGUISTIC.NOT_FOUND", "The specified linguistic entity was not found.");
}
